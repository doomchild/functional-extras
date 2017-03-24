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

      public class MaybeMap
      {
        private readonly string _defaultValue = "default";
        private Func<bool, string> _defaultMapper = b => b.ToString();

        [Fact]
        public void shouldApplyMapperForJust()
        {
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper, _testMaybe);
          string expectedResult = _testValue.ToString();

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnDefaultValueForNull()
        {
          Maybe<bool> testMaybe = null;
          string expectedResult = _defaultValue;
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper, testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnDefaultValueForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          string expectedResult = _defaultValue;
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper, testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class MaybeMapCurried
      {
        private string _defaultValue = "default";
        private Func<bool, string> _defaultMapper = b => b.ToString();

        [Fact]
        public void shouldApplyMapperForJust()
        {
          string expectedResult = _testValue.ToString();
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper)(_testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnDefaultValueForNull()
        {
          Maybe<bool> testMaybe = null;
          string expectedResult = _defaultValue;
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper)(testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnDefaultValueForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          string expectedResult = _defaultValue;
          string actualResult = Maybe<string>.MaybeMap(_defaultValue, _defaultMapper)(testMaybe);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class Of
      {
        [Fact]
        public void shouldReturnJustForValue()
        {
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = Maybe<bool>.Of(_testValue);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class OfNullable
      {
        [Fact]
        public void shouldReturnNothingForNull()
        {
          Maybe<object> expectedResult = Maybe<object>.Nothing<object>();
          Maybe<object> actualResult = Maybe<object>.OfNullable<object>(null);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnJustForValue()
        {
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = Maybe<bool>.OfNullable(_testValue);

          Assert.Equal(expectedResult, actualResult);
        }
      }
    }

    public class MaybeInstances
    {
      public class Alt
      {
        [Fact]
        public void shouldReturnNothingForBothNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Maybe<bool> testAlt = Maybe<bool>.Nothing<bool>();
          Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
          Maybe<bool> actualResult = testMaybe.Alt(testAlt);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnInstanceForNothingAlt()
        {
          Maybe<bool> testAlt = Maybe<bool>.Nothing<bool>();
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = _testMaybe.Alt(testAlt);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnAltForJustAlt()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Maybe<bool> testAlt = Maybe<bool>.Just(_testValue);
          Maybe<bool> expectedResult = testAlt;
          Maybe<bool> actualResult = testMaybe.Alt(testAlt);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnInstanceForJustInstance()
        {
          Maybe<bool> testAlt = Maybe<bool>.Just(!_testValue);
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = _testMaybe.Alt(testAlt);

          Assert.Equal(expectedResult, actualResult);
        }
      }
      public class Ap
      {
        [Fact]
        public void shouldApplyForJustOfValueAndFunction()
          {
            Maybe<Func<bool, bool>> testApply = Maybe<Func<bool, bool>>.Just<Func<bool, bool>>(b => !b);
            Maybe<bool> expectedResult = Maybe<bool>.Just(!_testValue);
            Maybe<bool> actualResult = _testMaybe.Ap(testApply);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotApplyForNothingOfFunction()
          {
            Maybe<Func<bool, bool>> testApply = Maybe<Func<bool, bool>>.Nothing<Func<bool, bool>>();
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = _testMaybe.Ap(testApply);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotApplyForNothingOfValue()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            Maybe<Func<bool, bool>> testApply = Maybe<bool>.Just<Func<bool, bool>>(b => !b);
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = testMaybe.Ap(testApply);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class Bind
      {
        [Fact]
        public void shouldAliasChain()
          {
            Func<bool, Maybe<bool>> testChain = value => Maybe<bool>.Just(!value);
            Maybe<bool> expectedResult = _testMaybe.Chain(testChain);
            Maybe<bool> actualResult = _testMaybe.Bind(testChain);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class Chain
      {
        [Fact]
        public void shouldChainForJust()
          {
            Func<bool, Maybe<bool>> testChain = value => Maybe<bool>.Just(!value);
            Maybe<bool> expectedResult = Maybe<bool>.Just(!_testValue);
            Maybe<bool> actualResult = _testMaybe.Chain(testChain);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotChainForNothing()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            Func<bool, Maybe<bool>> testChain = value => Maybe<bool>.Just(!value);
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = testMaybe.Chain(testChain);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class CheckedMap
      {
        private Func<bool, bool> _testMap = b => !b;
        private Func<bool, bool> _testThrowMap = value => throw new Exception();

        [Fact]
        public void shouldMapForJust()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Just(_testValue);
            Maybe<bool> expectedResult = Maybe<bool>.Just(!_testValue);
            Maybe<bool> actualResult = testMaybe.CheckedMap(_testMap);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotMapForNothing()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = testMaybe.CheckedMap(_testMap);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldMapThrownToNothing()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Just(_testValue);
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = testMaybe.CheckedMap(_testThrowMap);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class Coalesce
      {
        [Fact]
        public void shouldAliasAlt()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            Maybe<bool> expectedResult = testMaybe.Alt(_testMaybe);
            Maybe<bool> actualResult = testMaybe.Coalesce(_testMaybe);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class Extend
      {
        [Fact]
        public void shouldExtendForJust()
          {
            bool testDefaultValue = false;
            Func<Maybe<bool>, bool> testExtend = maybe => maybe
              //TODO(lee.crabtree): change to Predicates::Negate when you write it.
              .Map(b => !b)
              .GetOrElse(testDefaultValue);
            Maybe<bool> expectedResult = Maybe<bool>.Just(!_testValue);
            Maybe<bool> actualResult = _testMaybe.Extend(testExtend);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotExtendForNothing()
          {
            bool testDefaultValue = false;
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            Func<Maybe<bool>, bool> testExtend = maybe => maybe
              //TODO(lee.crabtree): change to Predicates::Negate when you write it.
              .Map(b => !b)
              .GetOrElse(testDefaultValue);
            Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
            Maybe<bool> actualResult = testMaybe.Extend(testExtend);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class Filter
      {
        [Fact]
        public void shouldFilterJustToJust()
          {
            string testValue = "test";
            Maybe<string> testMaybe = Maybe<string>.Just(testValue);
            //TODO(lee.crabtree): change to Predicates::AlwaysTrue when you write it.
            Predicate<string> testPredicate = s => true;
            Maybe<string> expectedResult = testMaybe;
            Maybe<string> actualResult = testMaybe.Filter(testPredicate);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldFilterJustToNothing()
          {
            string testValue = "test";
            Maybe<string> testMaybe = Maybe<string>.Just(testValue);
            //TODO(lee.crabtree): change to Predicates::AlwaysFalse when you write it.
            Predicate<string> testPredicate = s => false;
            Maybe<string> expectedResult = Maybe<string>.Nothing<string>();
            Maybe<string> actualResult = testMaybe.Filter(testPredicate);

            Assert.Equal(expectedResult, actualResult);
          }

        [Fact]
        public void shouldNotFilterNothingToJust()
          {
            Maybe<string> testMaybe = Maybe<string>.Nothing<string>();
            //TODO(lee.crabtree): change to Predicates::AlwaysFalse when you write it.
            Predicate<string> testPredicate = s => false;
            Maybe<string> expectedResult = testMaybe;
            Maybe<string> actualResult = testMaybe.Filter(testPredicate);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class FlatMap
      {
        [Fact]
        public void shouldAliasChain()
          {
            Func<bool, Maybe<bool>> testChain = value => Maybe<bool>.Just(!value);
            Maybe<bool> expectedResult = _testMaybe.Chain(testChain);
            Maybe<bool> actualResult = _testMaybe.FlatMap(testChain);

            Assert.Equal(expectedResult, actualResult);
          }
      }

      public class FoldLeft
      {
        private static readonly string _testInitialValue = "initialValue";
        private static readonly string _expectedResult = "expectedResult";
        private static readonly Func<string, bool, string> _testLeftFold = (initialValue, underlyingValue) => _expectedResult;

        [Fact]
        public void shouldReturnValueForJust()
          {
            string actualResult = _testMaybe.FoldLeft(_testLeftFold, _testInitialValue);

            Assert.Equal(_expectedResult, actualResult);
          }

        [Fact]
        public void shouldReturnValueForNothing()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            string actualResult = testMaybe.FoldLeft(_testLeftFold, _testInitialValue);

            Assert.Equal(_expectedResult, actualResult);
          }
      }

      public class FoldRight
      {
        private static readonly string _testInitialValue = "initialValue";
        private static readonly string _expectedResult = "expectedResult";
        private static readonly Func<bool, string, string> _testRightFold = (underlyingValue, initialValue) => _expectedResult;

        [Fact]
        public void shouldReturnValueForJust()
          {
            string actualResult = _testMaybe.FoldRight(_testRightFold, _testInitialValue);

            Assert.Equal(_expectedResult, actualResult);
          }

        [Fact]
        public void shouldReturnValueForNothing()
          {
            Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
            string actualResult = testMaybe.FoldRight(_testRightFold, _testInitialValue);

            Assert.Equal(_expectedResult, actualResult);
          }
      }

      public class GetOrElse
      {

      }

      public class GetOrElseGet
      {

      }

      public class GetOrElseThrow
      {

      }

      public class HashCode
      {

      }

      public class IfJust
      {

      }

      public class IfNothing
      {

      }

      public class IsJust
      {

      }

      public class IsNothing
      {

      }

      public class Map
      {

      }

      public class Recover
      {

      }

      public class Tap
      {

      }

      public class ToEither
      {

      }

      public class ToList
      {

      }

      public class ToNullable
      {

      }

      public class ToIEnumerable
      {

      }

      public class ToValidation
      {

      }
    }
  }
}
