namespace Piggyzen.Api.Models
{
    public enum CategoryGroupEnum
    {
        None = 0,          // Standardvärde, ingen specifik gruppering
        Villa = 1,         // För villarelaterade kategorier
        Hyresrätt = 2,     // För hyresrättsrelaterade kategorier
        Bostadsrätt = 3,   // För bostadsrättsrelaterade kategorier
        Fritidshus = 4,    // För fritidshusrelaterade kategorier
        Bil = 5,           // För bilrelaterade kategorier
        Båt = 6,           // För båtrelaterade kategorier
        Motorcykel = 7,    // För motorcykelrelaterade kategorier
        Husvagn = 8,       // För husvagnsrelaterade kategorier
    }
}