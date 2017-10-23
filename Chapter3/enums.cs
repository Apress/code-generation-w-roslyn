using System.ComponentModel;

namespace Chapter3
{
    public enum LoanPurpose
    {
        [Description("Launch a new business")] BusinessLaunching = 1,
        [Description("Buy a home")] HomePurchase = 2,
        [Description("Make home improvements")] HomeImprovement = 3,
        [Description("Finance investments")] Investment = 4,
        [Description("Cash out to pay off debts")] DebtConsolidation = 5,
        [Description("Finance education")] Education = 6,
        [Description("Emergency funds to handle an unforseen expense")] EmergencyExpendenture = 7,
        [Description("Buy a car")] CarPurchase = 8,
        [Description("Finance a wedding")] Wedding = 9,
        [Description("Finance travel plans")] Travel = 10
    }

    public enum Occupancy
    {
        [Description("Owner Occupied")] OwnerOccupied = 1,
        [Description("Second Home")] SecondHome = 2,
        [Description("Investment Property")] InvestmentProperty = 3
    }

    public enum PropertyTypes
    {
        [Description("Detached single family residence")] DetachedHouse = 1,
        [Description("Semi-detached houses with front, rear and any one side or both sides open")] SemiDetachedHouse = 2,
        [Description("An attached dwelling that is not a condo")] Townhome = 3,
        [Description("Single, individually-owned housing unit in a multi-unit building")] Condominium = 4,
        [Description("Multi floor row house with a brown sandstone facade")] BrownStone = 5
    }
}