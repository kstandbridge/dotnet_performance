using System.Runtime.CompilerServices;

namespace DotNetPerformance;

public class MonoPlayer
{
    private UInt64 _intellect;
    private UInt64 _spirit;
    private UInt64 _resource;
    public UInt64 Resource => _resource;
    public PlayerClassType PlayerClassType { get; private set; }

    public MonoPlayer(PlayerClassType playerClassType, UInt64 intellect, UInt64 spirit)
    {
        PlayerClassType = playerClassType;
        _intellect = intellect;
        _spirit = spirit;

        ClearResource();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt64 MaxResource()
    {
        UInt64 result = 0;

        switch(PlayerClassType)
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            {
                result = _intellect*10;
            } break;
            case PlayerClassType.Rogue:
            case PlayerClassType.Warrior:
            {
                result = 100;
            } break;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResourceTick()
    {
        switch(PlayerClassType)
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            {
                UInt64 maxResource = MaxResource();
                if(_resource < maxResource)
                {
                    UInt64 resourcePerTick = _spirit/5;
                    _resource += resourcePerTick;
                    if(_resource > maxResource)
                    {
                        _resource = maxResource;
                    }
                }
            } break;
            case PlayerClassType.Rogue:
            {
                UInt64 maxResource = MaxResource();
                if(_resource < maxResource)
                {
                    UInt64 resourcePerTick = 1;
                    _resource += resourcePerTick;
                    if(_resource > maxResource)
                    {
                        _resource = maxResource;
                    }
                }
            } break;
            case PlayerClassType.Warrior:
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
            } break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearResource()
    {
        switch(PlayerClassType)
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            case PlayerClassType.Rogue:
            {
                _resource = 0;
            } break;
            case PlayerClassType.Warrior:
            {
                _resource = 100;
            } break;
        }
    }
}
