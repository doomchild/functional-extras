using System;
using System.Collections.Generic;

using Xunit;

using FunctionalExtras.Category;

namespace FunctionalExtras.Tests
{
  public class MaybeTests
  {
    private static readonly bool _testValue = false;
    private static readonly Maybe<bool> _testMaybe = Maybe<bool>.Just(_testValue);

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

        [Fact]
        public void shouldReturnForNonNull()
        {
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = Maybe<bool>.From(_testValue);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnNothingForEmptyList()
        {
          List<object> testList = new List<object>();
          Maybe<object> expectedResult = Maybe<object>.Nothing<object>();
          Maybe<object> actualResult = Maybe<object>.From<object>(testList);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnJustOfFirstValueForPopulatedList()
        {
          List<bool> testList = new List<bool> { _testValue, !_testValue };
          Maybe<bool> expectedResult = Maybe<bool>.Just(testList[0]);
          Maybe<bool> actualResult = Maybe<bool>.From<bool>(testList);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnMaybeForMaybe()
        {
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = Maybe<bool>.From<bool>(_testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class FromJust
      {
        [Fact]
        public void shouldReturnValueForJust()
        {
          bool expectedResult = _testValue;
          bool actualResult = Maybe<bool>.FromJust(_testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldThrowForNothing()
        {
          Exception exception = Record.Exception(() => Maybe<bool>.FromJust(Maybe<bool>.Nothing<bool>()));
          Assert.NotNull(exception);
          Assert.IsType<ArgumentException>(exception);
        }
      }
    }
  }
}
