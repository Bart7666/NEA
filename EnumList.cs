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
    /// <summary>
    /// List of all Algorithms implemented, used to determine which one to execute
    /// </summary>
    public enum AlgorithmSelected
    {
        None,
        CaesarCipher,
        VigenèreCipher
    }
    
}

