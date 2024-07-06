using System.Collections.Generic;

public class Impacts
{
    public enum ImpactType { DEFAULT, BLOOD }

    public static Dictionary<ImpactType, string> IMPACT_POOLING = new Dictionary<ImpactType, string>()
    {
        { ImpactType.DEFAULT, "Normal_Impact"  },
        { ImpactType.BLOOD, "Blood_Impact"  }
    };
}