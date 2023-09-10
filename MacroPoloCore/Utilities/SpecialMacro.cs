namespace MacroPoloCore.Utilities
{
    public enum SpecialMacroType
    {
        IgnoreCase,         // ignore the key's case (a!)
        FirstCase,          // copy the first letter of key's case in the first letter of the value (a@)
        Pluralize,          // add an 's' to the end to key to get the plural version of the value (a$)
        FirstCasePluralize  // (a@$ or a$@)
    }

    internal class SpecialMacro
    { 
        public string value;
        public SpecialMacroType type;

        public override string ToString()
        {
            if (type == SpecialMacroType.IgnoreCase)
                return value + " (!)";
            else if (type == SpecialMacroType.FirstCase)
                return value + " (@)";
            else if (type == SpecialMacroType.Pluralize)
                return value + " (@)";
            else return value + " (@$)";
        }
    }
}
