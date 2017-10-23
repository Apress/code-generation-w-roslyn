namespace CommonProject
{

    public interface ILoanCodes
    {
        /// <summary> 
        /// Type of documentation used to verify income
        /// </summary>
        int Code1 { get; set; }
        /// <summary> 
        /// Number of Months Bank Statements are available
        /// </summary>
        int Code2 { get; set; }
        /// <summary> 
        /// Reason for this Loan
        /// </summary>
        int Code3 { get; set; }
        /// <summary> 
        /// Property Type
        /// </summary>
        int Code4 { get; set; }
        /// <summary> 
        /// Appraisal Type
        /// </summary>
        int Code5 { get; set; }
        /// <summary> 
        /// Number of comparable properites included in the appraisal
        /// </summary>
        int Code6 { get; set; }
        /// <summary> 
        /// Location
        /// </summary>
        int Code7 { get; set; }
        /// <summary> 
        /// Debt to Income Ratio
        /// </summary>
        decimal Code8 { get; set; }
        /// <summary> 
        /// Loan to Value Ratio
        /// </summary>
        decimal Code9 { get; set; }
        /// <summary> 
        /// The middle credit score for the primary borrower
        /// </summary>
        decimal Code10 { get; set; }
    }
}