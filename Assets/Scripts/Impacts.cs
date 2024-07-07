using System.Collections.Generic;

public class Impacts
{
    public enum ImpactType { DEFAULT, FLESH }

    public static Dictionary<ImpactType, string> IMPACT_POOLING = new Dictionary<ImpactType, string>()
    {
        { ImpactType.DEFAULT, "Normal_Impact"  },
        { ImpactType.FLESH, "Blood_Impact"  }
    };
}