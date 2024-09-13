using System.Collections.Generic;
using System.Linq;

public class DungeonInfoDict : DataDictionary<string, DungeonInfo>
{
    public DungeonInfoDict(DataSheetController controller) : base(controller, data => data.Rcode) { }
}