using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Moyo2_ItemFormChange
{
    public class CompPropertiesFormChange : CompProperties
    { 
		public List<TransformData> transformData = new List<TransformData>();

		public TransformData revertData;

		public List<string> sharedComps = new List<string>();

		private HashSet<Type> sharedCompsResolved;

		public bool onlyShowWhenDrafted;


		public CompPropertiesFormChange()
        {
            compClass = typeof(CompFormChange);
        }


        public HashSet<Type> SharedCompsResolved
        {
            get
            {
                bool flag = sharedCompsResolved == null;
                if (flag)
                {
                    sharedCompsResolved = [];
                    ResolveSharedComps();
                }
                return sharedCompsResolved;
            }
        }


        public void ResolveSharedComps()
        {
            for (int i = 0; i < sharedComps.Count; i++)
            {
                Type item = TypeFromString(sharedComps[i]);
                sharedCompsResolved.Add(item);
            }
        }


        public static Type TypeFromString(string typeString)
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
    }
}
