namespace NEA
{
    /// <summary>
    /// Datatype of the input, selected by user prior to encrypting / decrypting
    /// </summary>
    public enum DataInputType 
    {
        Text,
        TextFile,
        CSV
    }
    /// <summary>
    /// Result of attempting to validate input data
    /// </summary>
    public enum ValidationResult
    {
        Valid,
        KeyInvalid,
        DataInvalid,
        KeyAndDataInvalid
    }
    
}

