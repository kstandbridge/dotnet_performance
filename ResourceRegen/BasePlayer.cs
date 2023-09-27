using System.Runtime.CompilerServices;

namespace DotNetPerformance;

public abstract class BasePlayer
{
    protected UInt64 _intellect;
    protected UInt64 _spirit;
    protected UInt64 _resource;
    public UInt64 Resource => _resource;
    public virtual UInt64 MaxResource => _intellect*10;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ResourceTick()
    {
        UInt64 maxResource = MaxResource;
        if(_resource < maxResource)
        {
            UInt64 resourcePerTick = _spirit/5;
            _resource += resourcePerTick;
            if(_resource > maxResource)
            {
                _resource = maxResource;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void ClearResource()
    {
        _resource = 0;
    }
}

public class PriestPlayer : BasePlayer
{
    public PriestPlayer(UInt64 intellect, UInt64 spirit)
    {
        _intellect = intellect;
        _spirit = spirit;
    }
}

public class MagePlayer : BasePlayer
{
    public MagePlayer(UInt64 intellect, UInt64 spirit)
    {
        _intellect = intellect;
        _spirit = spirit;
    }
}

public class RoguePlayer : BasePlayer
{
    public override UInt64 MaxResource => UInt64.MaxValue;
    public RoguePlayer(UInt64 intellect, UInt64 spirit)
    {
        _intellect = intellect;
        _spirit = spirit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void ResourceTick()
    {
        UInt64 maxResource = MaxResource;
        if(_resource < maxResource)
        {
            UInt64 resourcePerTick = 1;
            _resource += resourcePerTick;
            if(_resource > maxResource)
            {
                _resource = maxResource;
            }
        }
    }
}

public class WarriorPlayer : BasePlayer
{
    public WarriorPlayer(UInt64 intellect, UInt64 spirit)
    {
        _intellect = intellect;
        _spirit = spirit;
        ClearResource();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void ResourceTick()
    {
        if(_resource > 0)
        {
            UInt64 resourcePerTick = 1;
            _resource -= resourcePerTick;
            if(_resource < 0)
            {
                _resource = 0;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void ClearResource()
    {
        _resource = UInt64.MaxValue;
    }
}
