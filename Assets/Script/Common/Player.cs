using System;

public interface Player
{
    double GetSensorInput(int idx);

    double GetPriority();

    double GetError();

    string GetUnitName();
}