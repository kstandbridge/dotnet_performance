using System.Runtime.CompilerServices;

namespace DotNetPerformance;

public class DataPlayer
{
    private Int32 _count;
    private UInt64[] _intellect;
    private UInt64[] _spirit;
    private UInt64[] _resource;
    public UInt64 Resource(Int32 index) => _resource[index];

    private PlayerClassType[] _classType;
    public PlayerClassType ClassType(Int32 index) => _classType[index];

    public DataPlayer(Int32 count)
    {
        _count = count;
        _intellect = new UInt64[count];
        _spirit = new UInt64[count];
        _resource = new UInt64[count];
        _classType = new PlayerClassType[count];
    }

    public void Add(Int32 index, PlayerClassType classType, UInt64 intellect, UInt64 spirit)
    {
        _classType[index] = classType;
        _intellect[index] = intellect;
        _spirit[index] = spirit;

        ClearResource(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UInt64 MaxResource(Int32 index)
    {
        UInt64 result = 0;

        switch(_classType[index])
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            {
                result = _intellect[index]*10;
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
    public void ResourceTick(Int32 index)
    {
        switch(_classType[index])
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            {
                UInt64 maxResource = MaxResource(index);
                if(_resource[index] < maxResource)
                {
                    UInt64 resourcePerTick = _spirit[index]/5;
                    _resource[index] += resourcePerTick;
                    if(_resource[index] > maxResource)
                    {
                        _resource[index] = maxResource;
                    }
                }
            } break;
            case PlayerClassType.Rogue:
            {
                UInt64 maxResource = MaxResource(index);
                if(_resource[index] < maxResource)
                {
                    UInt64 resourcePerTick = 1;
                    _resource[index] += resourcePerTick;
                    if(_resource[index] > maxResource)
                    {
                        _resource[index] = maxResource;
                    }
                }
            } break;
            case PlayerClassType.Warrior:
            {
                if(_resource[index] > 0)
                {
                    UInt64 resourcePerTick = 1;
                    _resource[index] -= resourcePerTick;
                    if(_resource[index] < 0)
                    {
                        _resource[index] = 0;
                    }
                }
            } break;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResourceTick()
    {
        for(Int32 index = 0; index < _count; ++index)
        {
            ResourceTick(index);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearResource(Int32 index)
    {
        switch(_classType[index])
        {
            case PlayerClassType.Priest:
            case PlayerClassType.Mage:
            case PlayerClassType.Rogue:
            {
                _resource[index] = 0;
            } break;
            case PlayerClassType.Warrior:
            {
                _resource[index] = 100;
            } break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearResource()
    {
        for(Int32 index = 0; index < _count; ++index)
        {
            ClearResource(index);
        }
    }
}
