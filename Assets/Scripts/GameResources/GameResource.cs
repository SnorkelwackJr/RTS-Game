using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResource
{
    private string _name;
    private int _currentAmount;

    public GameResource(string name, int initialAmount)
    {
        _name = name;
        _currentAmount = initialAmount;
    }

    public void AddAmount(int value)
    {
        _currentAmount += value;
        if (_currentAmount < 0) _currentAmount = 0;
    }

    public string Name { get => _name; }
    public int Amount { get => _currentAmount; set { _currentAmount = value; } }
}

[System.Serializable]
public class ResourceValue
{
    public string code = "";
    public int amount = 0;

    public ResourceValue(string code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}