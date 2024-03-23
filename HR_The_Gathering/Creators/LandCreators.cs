/*
jeroen visser 0952491
*/
namespace Creators;

// the new constraint here specifies that we can instantiate the colourtype with new() without parameters
abstract class LandCreator<LandColour> : CardCreator where LandColour : Colour, new()
{

    public override Land Create()
    {
        var id = this.GetId();
        var description = this.GetDescription();
        var colour = this.CreateColour();
        var createdCard = new Land(id, description, colour);
        return createdCard;
    }

    protected override Colour CreateColour()
    {
        return ColourCreator.GetColour<LandColour>();
    }

    // methods under here will never get invoked but otherwise the compiler will complain that we 
    // don't override all abstract inherited members
    protected override NoCost CreateEnergyCost()
    {
        return new NoCost(ColourCreator.GetColour<LandColour>());
    }

    protected override List<CardEffect> CreateEffects()
    {
        return new List<CardEffect> { };
    }
}

class MeadowCreator : LandCreator<Green>
{
    protected override string GetDescription()
    {
        return "a calm and soothing landscape";
    }

    protected override string GetId()
    {
        return "meadow";
    }
}

class VolcanoCreator : LandCreator<Red>
{
    protected override string GetDescription()
    {
        return "a fiery and rocky landscape";
    }

    protected override string GetId()
    {
        return "volcano";
    }
}

class OceanCreator : LandCreator<Blue>
{
    protected override string GetDescription()
    {
        return "massive body of salt water";
    }

    protected override string GetId()
    {
        return "ocean";
    }
}

class DesertCreator : LandCreator<Brown>
{
    protected override string GetDescription()
    {
        return "a landscape of sand and sunburn";
    }

    protected override string GetId()
    {
        return "desert";
    }
}

class SkyCreator : LandCreator<White>
{
    protected override string GetDescription()
    {
        return "sky filled with fluffy clouds";
    }

    protected override string GetId()
    {
        return "sky";
    }
}
