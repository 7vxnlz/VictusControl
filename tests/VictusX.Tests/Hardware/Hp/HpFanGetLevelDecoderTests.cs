using GHelper.Hardware.Hp;
using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpFanGetLevelDecoderTests
{
    [Fact]
    public void Decode_With128ByteInput_SucceedsWithRawValuesOnly()
    {
        byte[] sample = new byte[HpFanGetLevelDecoder.ExpectedLength];
        sample[0] = 0x21;
        sample[1] = 0x2A;

        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode(sample);

        Assert.True(decoded.IsValid);
        Assert.Equal(128, decoded.Length);
        Assert.Equal(128, decoded.ExpectedLength);
        Assert.Equal(string.Empty, decoded.ValidationError);
        Assert.Equal<byte?>(0x21, decoded.Fan1RawValue);
        Assert.Equal<byte?>(0x2A, decoded.Fan2RawValue);
        Assert.Equal(new byte[] { 0x21, 0x2A }, decoded.RawValueBytes);
        Assert.Equal("21-2A", decoded.KnownPrefixHex);
        Assert.Equal(126, decoded.UnknownByteCount);
        Assert.Equal("2..127", decoded.UnknownByteRange);
        Assert.Equal(0, decoded.UnknownNonZeroByteCount);
    }

    [Fact]
    public void Decode_WithTooShortInput_FailsSafely()
    {
        byte[] sample = [0x21];

        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode(sample);

        Assert.False(decoded.IsValid);
        Assert.Equal(1, decoded.Length);
        Assert.Contains("at least 2 bytes", decoded.ValidationError, StringComparison.Ordinal);
        Assert.Null(decoded.Fan1RawValue);
        Assert.Null(decoded.Fan2RawValue);
        Assert.Empty(decoded.RawValueBytes);
        Assert.Equal(0, decoded.UnknownByteCount);
    }

    [Fact]
    public void Decode_WithEmptyInput_FailsSafely()
    {
        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode([]);

        Assert.False(decoded.IsValid);
        Assert.Equal(0, decoded.Length);
        Assert.Contains("at least 2 bytes", decoded.ValidationError, StringComparison.Ordinal);
        Assert.Null(decoded.Fan1RawValue);
        Assert.Null(decoded.Fan2RawValue);
        Assert.Empty(decoded.RawValueBytes);
    }

    [Fact]
    public void Decode_WithNullInput_FailsSafely()
    {
        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode(null);

        Assert.False(decoded.IsValid);
        Assert.Equal(0, decoded.Length);
        Assert.Contains("null", decoded.ValidationError, StringComparison.Ordinal);
        Assert.Null(decoded.Fan1RawValue);
        Assert.Null(decoded.Fan2RawValue);
        Assert.Empty(decoded.RawValueBytes);
    }

    [Fact]
    public void Decode_WithUnknownTail_SummarizesOnly()
    {
        byte[] sample = new byte[HpFanGetLevelDecoder.ExpectedLength];
        sample[0] = 0x10;
        sample[1] = 0x11;
        sample[2] = 0xAA;
        sample[64] = 0x55;
        sample[127] = 0x01;

        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode(sample);

        Assert.True(decoded.IsValid);
        Assert.Equal<byte?>(0x10, decoded.Fan1RawValue);
        Assert.Equal<byte?>(0x11, decoded.Fan2RawValue);
        Assert.Equal(126, decoded.UnknownByteCount);
        Assert.Equal("2..127", decoded.UnknownByteRange);
        Assert.Equal(3, decoded.UnknownNonZeroByteCount);
    }

    [Fact]
    public void Decode_WithNonExpectedButKnownPrefixLength_DecodesRawPrefixWithValidationWarning()
    {
        byte[] sample = [0x10, 0x11, 0x22];

        HpFanGetLevelSnapshot decoded = HpFanGetLevelDecoder.Decode(sample);

        Assert.True(decoded.IsValid);
        Assert.Contains("Expected 128 bytes", decoded.ValidationError, StringComparison.Ordinal);
        Assert.Equal<byte?>(0x10, decoded.Fan1RawValue);
        Assert.Equal<byte?>(0x11, decoded.Fan2RawValue);
        Assert.Equal(1, decoded.UnknownByteCount);
        Assert.Equal("2..2", decoded.UnknownByteRange);
        Assert.Equal(1, decoded.UnknownNonZeroByteCount);
    }
}
