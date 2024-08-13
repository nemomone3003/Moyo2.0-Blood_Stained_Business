using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Moyo2_ItemFormChange
{
    // Token: 0x02000004 RID: 4
    public class CompPropertiesFormChange : CompProperties
    {
        // Token: 0x06000005 RID: 5 RVA: 0x00002140 File Offset: 0x00000340
        public CompPropertiesFormChange()
        {
            compClass = typeof(CompFormChange);
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000006 RID: 6 RVA: 0x00002170 File Offset: 0x00000370
        public HashSet<Type> SharedCompsResolved
        {
            get
            {
                bool flag = sharedCompsResolved == null;
                if (flag)
                {
                    sharedCompsResolved = new HashSet<Type>();
                    resolveSharedComps();
                }
                return sharedCompsResolved;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000021AC File Offset: 0x000003AC
        public void resolveSharedComps()
        {
            for (int i = 0; i < sharedComps.Count; i++)
            {
                Type item = typeFromString(sharedComps[i]);
                sharedCompsResolved.Add(item);
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000021F8 File Offset: 0x000003F8
        public static Type typeFromString(string typeString)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeString, false, true);
                bool flag = type != null;
                if (flag)
                {
                    return type;
                }
            }
            Type type2 = Type.GetType(typeString, false, true);
            bool flag2 = type2 != null;
            if (flag2)
            {
                return type2;
            }
            return null;
        }

        // Token: 0x04000008 RID: 8
        public List<TransformData> transformData = new List<TransformData>();

        // Token: 0x04000009 RID: 9
        public TransformData revertData;

        // Token: 0x0400000A RID: 10
        public List<string> sharedComps = new List<string>();

        // Token: 0x0400000B RID: 11
        private HashSet<Type> sharedCompsResolved;

        public bool onlyShowWhenDrafted;
    }
}
