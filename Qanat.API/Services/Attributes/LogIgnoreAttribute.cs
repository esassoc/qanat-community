using System;

namespace Qanat.API.Services.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class LogIgnoreAttribute : Attribute
{
    public LogIgnoreAttribute() {}
}