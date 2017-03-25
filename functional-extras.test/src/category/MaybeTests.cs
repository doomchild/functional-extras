using System;
using System.Collections.Generic;

using Xunit;
using Moq;

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
        [Fact]
        public void shouldReturnValueForJust()
        {
          bool testDefaultValue = !_testValue;
          bool expectedResult = _testValue;
          bool actualResult = _testMaybe.GetOrElse(testDefaultValue);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnOtherValueForNothing()
        {
          bool testDefaultValue = !_testValue;
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          bool expectedResult = !_testValue;
          bool actualResult = testMaybe.GetOrElse(testDefaultValue);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class GetOrElseGet
      {
        [Fact]
        public void shouldReturnValueForJust()
        {
          //TODO(lee.crabtree): replace with Suppliers.Always(!testValue) when I write it.
          Func<bool> testSupplier = () => !_testValue;
          bool expectedResult = _testValue;
          bool actualResult = _testMaybe.GetOrElseGet(testSupplier);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnOtherValueForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          //TODO(lee.crabtree): replace with Suppliers.Always(!testValue) when I write it.
          Func<bool> testSupplier = () => !_testValue;
          bool expectedResult = !_testValue;
          bool actualResult = testMaybe.GetOrElseGet(testSupplier);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class GetOrElseThrow
      {
        [Fact]
        public void shouldReturnValueForJust()
        {
          Func<Exception> testSupplier = () => new Exception();
          bool expectedResult = _testValue;
          bool actualResult = _testMaybe.GetOrElseThrow(testSupplier);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldThrowForNothing()
        {
          Func<Exception> testSupplier = () => new Exception();
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Exception exception = Record.Exception(() => testMaybe.GetOrElseThrow(testSupplier));

          Assert.NotNull(exception);
          Assert.IsType<Exception>(exception);
        }
      }

      public class HashCode
      {
        [Fact]
        public void shouldReturn1ForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          int expectedResult = 1;
          int actualResult = testMaybe.GetHashCode();

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnValueHashCodeForJust()
        {
          int expectedResult = _testValue.GetHashCode();
          int actualResult = _testMaybe.GetHashCode();

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class IfJust
      {
        [Fact]
        public void shouldCallIfJustForJust()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Just(_testValue);
          Mock<Action<bool>> testConsumer = new Mock<Action<bool>>();
          int expectedInvocations = 1;

          testMaybe.IfJust(testConsumer.Object);

          testConsumer.Verify(f => f(_testValue), Times.Exactly(expectedInvocations));
        }

        [Fact]
        public void shouldNotCallIfJustForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Mock<Action<bool>> testConsumer = new Mock<Action<bool>>();
          int expectedInvocations = 0;

          testMaybe.IfJust(testConsumer.Object);

          testConsumer.Verify(f => f(_testValue), Times.Exactly(expectedInvocations));
        }
      }

      public class IfNothing
      {
        [Fact]
        public void shouldCallIfNothingForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Mock<Action> testRunnable = new Mock<Action>();
          int expectedInvocations = 1;

          testMaybe.IfNothing(testRunnable.Object);

          testRunnable.Verify(f => f(), Times.Exactly(expectedInvocations));
        }

        [Fact]
        public void shouldNotCallIfNothingForJust()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Just(_testValue);
          Mock<Action> testRunnable = new Mock<Action>();
          int expectedInvocations = 0;

          testMaybe.IfNothing(testRunnable.Object);

          testRunnable.Verify(f => f(), Times.Exactly(expectedInvocations));
        }
      }

      public class IsJust
      {
        [Fact]
        public void shouldReturnTrueForJust()
        {
          Assert.True(_testMaybe.IsJust());
        }

        [Fact]
        public void shouldReturnFalseForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();

          Assert.False(testMaybe.IsJust());
        }
      }

      public class IsNothing
      {
        [Fact]
        public void shouldReturnFalseForJust()
        {
          Assert.False(_testMaybe.IsNothing());
        }

        [Fact]
        public void shouldReturnTrueForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();

          Assert.True(testMaybe.IsNothing());
        }
      }

      public class Map
      {
        //TODO(lee.crabtree): replace with Predicates::Negate when I write it.
        private readonly Func<bool, bool> _testMap = b => !b;

        [Fact]
        public void shouldMapForJust()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Just(_testValue);
          Maybe<bool> expectedResult = Maybe<bool>.Just(!_testValue);
          Maybe<bool> actualResult = testMaybe.Map(_testMap);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldNotMapForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Maybe<bool> expectedResult = Maybe<bool>.Nothing<bool>();
          Maybe<bool> actualResult = testMaybe.Map(_testMap);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class Recover
      {
        private readonly bool _testRecover = !_testValue;

        [Fact]
        public void shouldReturnValueForJust()
        {
          Maybe<bool> expectedResult = _testMaybe;
          Maybe<bool> actualResult = _testMaybe.Recover(_testRecover);

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnValueForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          Maybe<bool> expectedResult = Maybe<bool>.Just(_testRecover);
          Maybe<bool> actualResult = testMaybe.Recover(_testRecover);

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class Tap
      {

      }

      public class ToEither
      {

      }

      public class ToList
      {
        [Fact]
        public void shouldReturnListForJust()
        {
          IList<bool> expectedResult = new List<bool> { _testValue };
          IList<bool> actualResult = _testMaybe.ToList();

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnEmptyListForNothing()
        {
          Maybe<bool> testMaybe = Maybe<bool>.Nothing<bool>();
          IList<bool> expectedResult = new List<bool>();
          IList<bool> actualResult = testMaybe.ToList();

          Assert.Equal(expectedResult, actualResult);
        }
      }

      public class ToValidation
      {

      }

      public class String
      {
        [Fact]
        public void shouldReturnMaybeValueForJust()
        {
          string expectedResult = "Maybe{" + _testValue + "}";
          string actualResult = _testMaybe.ToString();

          Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void shouldReturnNothingForNothing()
        {
          Maybe<object> testMaybe = Maybe<object>.Nothing<object>();
          string expectedResult = "Nothing";
          string actualResult = testMaybe.ToString();

          Assert.Equal(expectedResult, actualResult);
        }
      }
    }
  }
}
