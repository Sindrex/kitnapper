using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitnapper/MultiRequiredFlag")]
[Serializable]
public class MultiReqFlag : ReqFlagBase
{
    public List<ReqFlagBase> ReqFlags;

    public override GameFlag GetFlag() => ReqFlags.First().GetFlag(); //kinda wonky?

    public override bool Result()
    {
        return ReqFlags.All(x => x.Result());
    }
}