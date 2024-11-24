namespace Piggyzen.Api.Models
{
    public enum VerificationStatusEnum
    {
        Unverified,          // Ingen verifiering har utförts
        FlaggedForReview,    // Flagga för misstänkt dublett och kräver granskning
        VerifiedAsDuplicate, // Verifierad som dublett
        VerifiedAsUnique     // Verifierad som unik
    }
}