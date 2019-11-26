namespace LibSvnSharp
{
    public enum SvnLineStyle
    {
        Default = 0,
        CarriageReturnLinefeed,
        Linefeed,
        CarriageReturn,
        Native,
        Windows = CarriageReturnLinefeed,
        Unix = Linefeed
    }
}
