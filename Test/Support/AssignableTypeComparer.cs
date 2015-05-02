namespace Microsoft.PackageManagement.SwidTag.Test.Support {
    using System;
    using System.Collections.Generic;

    public class AssignableTypeComparer : IEqualityComparer<Type> {
        public static readonly AssignableTypeComparer Instance = new AssignableTypeComparer();

        public bool Equals(Type x, Type y) {
            return IsAssignableOrCompatible(x, y);
        }

        public int GetHashCode(Type obj) {
            // unused.
            return -1;
        }

        public static bool IsAssignableOrCompatible(Type x, Type y) {
            if (x == null) {
                return y == null;
            }
            // adding support to see if we can duck-type the target type to the correct type.
            return x == y || x.IsAssignableFrom(y);
        }
    }
}