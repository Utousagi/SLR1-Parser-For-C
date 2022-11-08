using System.Collections;

namespace CCompiler.utils;

public class MCharEnumerator : IEnumerator<char>, ICloneable
{
    private string? _str;
    private int _record;
    private int _index;
    private char _current;
    
    public MCharEnumerator(string str)
    {
        _str = str;
        _record = -1;
        _index = -1;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public bool MoveNext()
    {
        if (_index < _str!.Length - 1)
        {
            _index++;
            _current = _str[_index];
            return true;
        }
        _index = _str.Length;
        return false;
    }

    public bool MovePre()
    {
        if (_index > 0)
        {
            _index--;
            _current = _str![_index];
            return true;
        } 
        _index = -1;
        return false;
    }

    public void Dispose()
    {
        if (_str != null)
            _index = _str.Length;
        _str = null;
        GC.SuppressFinalize(this);
    }
    
    object IEnumerator.Current => Current;

    public char Current
    {
        get
        {
            if (_index == -1)
                throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
            if (_index >= _str!.Length)
                throw new InvalidOperationException("InvalidOperation_EnumEnded");
            return _current;
        }
    }
    
    public void Reset()
    {
        _current = default;
        _record = -1;
        _index = -1;
    }
    
    public void Record()
    {
        _record = _index;
    }
    
    public void Rollback()
    {
        _index = _record;
        _current = _str![_index];
    }
}