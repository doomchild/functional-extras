using System;

namespace FunctionalExtras.Category
{
  public static class Objects
  {
    public static T RequireNonNull<T>(T obj, string errorString)
    {
      if(obj == null)
      {
        throw new ArgumentNullException(errorString);
      }

      return obj;
    }
  }
}
