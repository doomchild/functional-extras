using System;
using Xunit;

using FunctionalExtras.Category;

namespace FunctionalExtras.Tests
{
  public class MaybeTests
  {
    private static bool _testValue = false;

    public class MaybeStatics
    {
      public class Attempt
      {
        [Fact]
        public void ShouldReturnRightOfTheValue()
        {
          Func<bool> testSupplier = () => _testValue;
          Maybe<bool> expectedResult = Maybe<bool>.Just(_testValue);
          Maybe<bool> actualResult = Maybe<bool>.Attempt(testSupplier);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void ShouldReturnLeftOfTheThrowable()
        {
          Func<bool> testSupplier = () => { throw new Exception(); };
          Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
          Maybe<bool> actualResult = Maybe<bool>.Attempt(testSupplier);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class Empty
      {
        [Fact]
        public void ShouldReturnNothing()
        {
          Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
          Maybe<bool> actualResult = Maybe<bool>.Empty<bool>();

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class From
      {
        [Fact]
        public void ShouldReturnNothingForNull()
        {
          object testValue = null;
          Maybe<object> expectedResult = Maybe<object>.Nothing<object>();
          Maybe<object> actualResult = Maybe<object>.From(testValue);

          Assert.Equal(expectedResult, actualResult);
        }
      }
    }
  }
}
