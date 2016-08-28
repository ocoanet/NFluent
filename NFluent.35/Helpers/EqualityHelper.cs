﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="EqualityHelper.cs" company="">
// //   Copyright 2013 Thomas PIERRAIN
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NFluent.Helpers
{
    using NFluent.Extensibility;

    /// <summary>
    /// Helper class related to Equality methods (used like a traits).
    /// </summary>
    internal static class EqualityHelper
    {
        /// <summary>
        /// Checks that a given instance is considered to be equal to another expected instance. Throws <see cref="FluentCheckException"/> otherwise.
        /// </summary>
        /// <typeparam name="T">Checked type.
        /// </typeparam>
        /// <typeparam name="TU">Checker type.
        /// </typeparam>
        /// <param name="checker">
        /// The checker.
        /// </param>
        /// <param name="expected">
        /// The expected instance.
        /// </param>
        /// <exception cref="FluentCheckException">
        /// The actual value is not equal to the expected value.
        /// </exception>
        public static void IsEqualTo<T, TU>(IChecker<T, TU> checker, object expected) where TU : class, IMustImplementIForkableCheckWithoutDisplayingItsMethodsWithinIntelliSense
        {
            var instance = checker.Value;
            if (FluentEquals(instance, expected))
            {
                return;
            }
            
            // Should throw
            var errorMessage = BuildErrorMessage(checker, expected, false);

            throw new FluentCheckException(errorMessage);
        }

        private static bool FluentEquals<T>(T instance, object expected)
        {
            // Fast and clean path that should be taken most of the time
            if (expected is T)
                return EqualityComparer<T>.Default.Equals(instance, (T) expected);

            return FluentEquals((object) instance, expected);
        }

        /// <summary>
        /// Determines whether the instance is equal to the expected value, using the correct Equals method overload.
        /// </summary>
        /// <param name="instance">The value to be tested.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>true if the instance is equal to the expected value; otherwise, false.</returns>
        public static bool FluentEquals(object instance, object expected)
        {
            if (expected != null && instance != null)
            {
                var equatableType = typeof(IEquatable<>).MakeGenericType(expected.GetType());
                if (equatableType.IsInstanceOfType(instance))
                    return (bool)equatableType.GetMethod("Equals").Invoke(instance, new[] { expected });
            }

            return object.Equals(instance, expected);
        }

        /// <summary>
        /// Builds the error message related to the Equality verification. This should be called only if the test failed (no matter it is negated or not).
        /// </summary>
        /// <typeparam name="T">Checked type.
        /// </typeparam>
        /// <typeparam name="TU">Checker type.</typeparam>
        /// <param name="checker">
        /// The checker.
        /// </param>
        /// <param name="expected">
        /// The other operand.
        /// </param>
        /// <param name="isEqual">
        /// A value indicating whether the two values are equal or not. <c>true</c> if they are equal; <c>false</c> otherwise.
        /// </param>
        /// <returns>
        /// The error message related to the Equality verification.
        /// </returns>
        public static string BuildErrorMessage<T, TU>(IChecker<T, TU> checker, object expected, bool isEqual) where TU : class, IMustImplementIForkableCheckWithoutDisplayingItsMethodsWithinIntelliSense
        {
            var msg = isEqual ? checker.BuildShortMessage("The {0} is equal to the {1} whereas it must not.") : checker.BuildShortMessage("The {0} is different from the {1}.");

            FillEqualityErrorMessage(msg, checker.Value, expected, isEqual);

            return msg.ToString();
        }

        public static void FillEqualityErrorMessage(FluentMessage msg, object instance, object expected, bool negated)
        {
            if (negated)
            {
                msg.Expected(expected).Comparison("different from").WithType();
                return;
            }

            // shall we display the type as well?
            var withType = (instance != null && expected != null && instance.GetType() != expected.GetType())
                           || (instance == null);

            // shall we display the hash too
            var withHash = instance != null && expected != null && instance.GetType() == expected.GetType()
                           && instance.ToString() == expected.ToString();

            msg.On(instance)
                .WithType(withType)
                .WithHashCode(withHash)
                .And.Expected(expected)
                .WithType(withType)
                .WithHashCode(withHash);
            return;
        }

        /// <summary>
        /// Checks that a given instance is not considered to be equal to another expected instance. Throws <see cref="FluentCheckException"/> otherwise.
        /// </summary>
        /// <typeparam name="T">Checked type.
        /// </typeparam>
        /// <typeparam name="TU">Checker type.
        /// </typeparam>
        /// <param name="checker">The checker.</param>
        /// <param name="expected">The expected instance.</param>
        /// <exception cref="FluentCheckException">The actual value is not equal to the expected value.</exception>
        public static void IsNotEqualTo<T, TU>(IChecker<T, TU> checker, object expected) where TU : class, IMustImplementIForkableCheckWithoutDisplayingItsMethodsWithinIntelliSense
        {
            if (FluentEquals(checker.Value, expected))
            {
                throw new FluentCheckException(BuildErrorMessage(checker, expected, true));
            }
        }
    }
}