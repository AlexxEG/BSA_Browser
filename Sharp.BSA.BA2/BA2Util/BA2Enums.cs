namespace SharpBSABA2.BA2Util
{
    public enum BA2Errors
    {
        ERR_UNK = 0, //because we want 1 = ok
        ERR_OK,
        ERR_FILE_NOT_FOUND,
        ERR_NOT_BA2, //magic is incorrect
        ERR_MALFORMED_FILE, //file is shorter than nametable offset
    }

    public enum BA2HeaderMagic
    {
        BTDX,
        Unknown
    }

    public enum BA2HeaderType
    {
        GNRL,
        DX10,
        Unknown
    }
}
