﻿using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace TellagoStudios.Hermes.Client.Util
{
    public class Guard
    {
        static public readonly Guard Instance = new Guard();

        private Guard()
        {
        }

        public Guard ArgumentNotNull(Expression<Func<object>> argName, object argValue)
        {
            if (argValue == null)
                throw new ArgumentNullException(GetArgumentName(argName));

            return Instance;
        }

        public Guard ArgumentNotNullOrEmpty(Expression<Func<object>> argName, IEnumerable argValue)
        {
            if (argValue == null)
                throw new ArgumentNullException(GetArgumentName(argName));

            var strValue = argValue as string;
            if (!string.IsNullOrEmpty(strValue))
                return Instance;
            
            if (argValue.Cast<object>().Any())
                return Instance;

            throw new ArgumentException(Messages.ArgumentWasEmpty, GetArgumentName(argName));
        }

        public Guard ArgumentNotNullOrWhiteSpace(Expression<Func<object>> argName, string argValue)
        {
            if (argValue == null)
                throw new ArgumentNullException(GetArgumentName(argName));

            if (!string.IsNullOrWhiteSpace(argValue))
                return Instance;

            throw new ArgumentException(Messages.ArgumentWasEmptyOrWhitespace, GetArgumentName(argName));
        }
 
        public Guard ArgumentValid(Expression<Func<object>> argName, Func<bool> validation)
        {
            if (validation())
                throw new ArgumentException(Messages.ArgumentWasInvalid, GetArgumentName(argName));

            return Instance;
        }

        static private string GetArgumentName(Expression<Func<object>> exp)
        {
            return GetArgumentName(exp.Body);
        }

        static private string GetArgumentName(Expression exp)
        {
            var member = exp as MemberExpression;
            if (member != null) return member.Member.Name;

            var unary = exp as UnaryExpression;
            if (unary != null) return GetArgumentName(unary.Operand);

            var lambda = exp as LambdaExpression;
            if (lambda != null) return GetArgumentName(lambda.Body);

            return "unknown";
        }
    }
}
