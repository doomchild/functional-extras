using System;
using System.Collections.Generic;

namespace FunctionalExtras.Category
{
  public class Maybe<V>
  {
    private readonly V _value;
    private readonly bool _isNothing;

    private Maybe(V val, bool isNothing)
    {
      _value = val;
      _isNothing = isNothing;
    }

    public static Maybe<V> Attempt(Func<V> supplier)
    {
      try
      {
        return Just(supplier());
      }
      catch(Exception)
      {
        return Nothing<V>();
      }
    }

    public static Maybe<R> Empty<R>()
    {
      return Nothing<R>();
    }

    public static Maybe<R> From<R>(R value)
    {
      return (value == null)
        ? Nothing<R>()
        : Just(value);
    }

    public static Maybe<R> From<R>(Maybe<R> maybe)
    {
      return maybe ?? Nothing<R>();
    }

    public static Maybe<R> From<R>(IList<R> list)
    {
      return (list == null || list.Count <= 0)
        ? Nothing<R>()
        : OfNullable(list[0]);
    }

    public static R FromJust<R>(Maybe<R> maybe)
    {
      Objects.RequireNonNull(maybe, "maybe must not be null");

      if(maybe.IsNothing())
      {
        throw new ArgumentException("maybe must not be Nothing");
      }

      return maybe._value;
    }

    public static Maybe<R> Just<R>(R value)
    {
      Objects.RequireNonNull(value, "value must not be null");

      return new Maybe<R>(value, false);
    }

    public static Func<Maybe<T>, R> MaybeMap<T, R>(R defaultValue, Func<T, R> mapper)
    {
      return instance => MaybeMap<T, R>(defaultValue, mapper, instance);
    }

    public static R MaybeMap<T, R>(R defaultValue, Func<T, R> mapper, Maybe<T> maybe)
    {
      if(maybe != null && maybe.IsJust())
      {
        return mapper(maybe._value);
      }
      else
      {
        return defaultValue;
      }
    }

    public static Maybe<R> Nothing<R>()
    {
      return new Maybe<R>(default(R), true);
    }

    public static Maybe<R> Of<R>(R value)
    {
      return Just(value);
    }

    public static Maybe<R> OfNullable<R>(R value)
    {
      return (value == null)
        ? Nothing<R>()
        : Just(value);
    }

    public Maybe<V> Alt(Maybe<V> other)
    {
      Objects.RequireNonNull(other, "other must not be null");

      return this.IsJust()
        ? this
        : other;
    }

    public Maybe<R> Ap<R>(Maybe<Func<V, R>> other)
    {
      Objects.RequireNonNull(other, "other must not be null");

      if(other.IsJust())
      {
        Maybe<Maybe<R>> maybe = other.Map<Maybe<R>>(Map);

        return maybe.IsJust()
          ? FromJust(maybe)
          : Nothing<R>();
      }
      else
      {
        return Nothing<R>();
      }
    }

    public Maybe<R> Bind<R>(Func<V, Maybe<R>> mapper)
    {
      return this.Chain(mapper);
    }

    public Maybe<R> Chain<R>(Func<V, Maybe<R>> mapper)
    {
      Objects.RequireNonNull(mapper, "mapper must not be null");

      if(IsJust())
      {
        Maybe<Maybe<R>> maybe = Map<Maybe<R>>(mapper);

        return maybe.IsJust()
          ? FromJust(maybe)
          : Nothing<R>();
      }
      else
      {
        return Nothing<R>();
      }
    }

    public Maybe<R> CheckedMap<R>(Func<V, R> mapper)
    {
      try
      {
        return IsJust()
          ? OfNullable(mapper(_value))
          : Nothing<R>();
      } catch
      {
        return Nothing<R>();
      }
    }

    public Maybe<V> Coalesce(Maybe<V> other)
    {
      return Alt(other);
    }

    public Maybe<R> Extend<R>(Func<Maybe<V>, R> mapper)
    {
      Objects.RequireNonNull(mapper, "mapper must not be null");

      return IsJust()
        ? Duplicate().Map(mapper)
        : Nothing<R>();
    }

    public Maybe<V> Filter(Predicate<V> predicate)
    {
      Objects.RequireNonNull(predicate, "predicate must not be null");

      return IsJust() && predicate(_value)
        ? this
        : Nothing<V>();
    }

    public Maybe<R> FlatMap<R>(Func<V, Maybe<R>> mapper)
    {
      return Chain(mapper);
    }

    public R FoldLeft<R>(Func<R, V, R> morphism, R initialValue)
    {
      Objects.RequireNonNull(morphism, "morphism must not be null");

      return morphism(initialValue, _value);
    }

    public R FoldRight<R>(Func<V, R, R> morphism, R initialValue)
    {
      Objects.RequireNonNull(morphism, "morphism must not be null");

      return morphism(_value, initialValue);
    }

    public V GetOrElse(V otherValue)
    {
      return IsJust()
        ? _value
        : otherValue;
    }

    public V GetOrElseGet(Func<V> supplier)
    {
      Objects.RequireNonNull(supplier, "supplier must not be null");

      return IsJust()
        ? _value
        : supplier();
    }

    public V GetOrElseThrow<E>(Func<E> supplier) where E : Exception
    {
      Objects.RequireNonNull(supplier, "supplier must not be null");

      if(IsJust())
      {
        return _value;
      }
      else
      {
        throw supplier();
      }
    }

    public Maybe<V> IfJust(Action<V> consumer)
    {
      Objects.RequireNonNull(consumer, "consumer must not be null");

      if(IsJust())
      {
        consumer(_value);
      }

      return this;
    }

    public Maybe<V> IfNothing(Action runnable)
    {
      Objects.RequireNonNull(runnable, "runnable must not be null");

      if(IsNothing())
      {
        runnable();
      }

      return this;
    }

    public bool IsJust()
    {
      return !_isNothing;
    }

    public bool IsNothing()
    {
      return _isNothing;
    }

    public Maybe<R> Map<R>(Func<V, R> mapper)
    {
      Objects.RequireNonNull(mapper, "mapper must not be null");

      R val = mapper(_value);

      return IsJust()
        ? OfNullable(val)
        : Nothing<R>();
    }

    public Maybe<V> Recover(V value)
    {
      Objects.RequireNonNull(value, "value must not be null");

      return IsJust()
        ? this
        : Just(value);
    }

    public Maybe<V> Tap(Action runnable, Action<V> consumer)
    {
      Objects.RequireNonNull(runnable, "runnable must not be null");
      Objects.RequireNonNull(consumer, "consumer must not be null");

      return IfNothing(runnable)
        .IfJust(consumer);
    }

    public override int GetHashCode()
    {
      return _value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Maybe<V> maybe = obj as Maybe<V>;

      if(maybe == null)
      {
        return false;
      }

      if(maybe.IsNothing() && this.IsNothing())
      {
        return true;
      }

      return maybe._value.Equals(this._value);
    }

    public override string ToString()
    {
      return _isNothing
        ? "Nothing"
        : "Maybe{" + _value + "}";
    }

    private Maybe<Maybe<V>> Duplicate()
    {
      return Just(this);
    }
  }
}
