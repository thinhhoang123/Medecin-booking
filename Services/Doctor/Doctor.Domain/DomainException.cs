using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Doctor.Domain
{
    /// <summary>
    /// Base exception for domain violations
    /// </summary>
    public class DomainException : Exception
    {

        public string Code { get; }
        public HttpStatusCode StatusCode { get; }
        public Dictionary<string, object>? AdditionalData { get; }

        public DomainException(string message)
            : base(message)
        {
            Code = "DOMAIN_ERROR";
            StatusCode = HttpStatusCode.BadRequest;
        }

        public DomainException(string message, string code)
            : base(message)
        {
            Code = code;
            StatusCode = HttpStatusCode.BadRequest;
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
            Code = "DOMAIN_ERROR";
            StatusCode = HttpStatusCode.BadRequest;
        }

        public DomainException(
            string message,
            string code,
            HttpStatusCode statusCode)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public DomainException(
            string message,
            string code,
            HttpStatusCode statusCode,
            Dictionary<string, object> additionalData)
            : base(message)
        {
            Code = code;
            StatusCode = statusCode;
            AdditionalData = additionalData;
        }

        public DomainException(
            string message,
            string code,
            HttpStatusCode statusCode,
            Exception innerException)
            : base(message, innerException)
        {
            Code = code;
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Thrown when entity validation fails
    /// </summary>
    public class ValidationException : DomainException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(string message)
            : base(message, "VALIDATION_ERROR", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation failed", "VALIDATION_ERROR", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string message, Dictionary<string, string[]> errors)
            : base(message, "VALIDATION_ERROR", HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }

        public ValidationException(string propertyName, string errorMessage)
            : base("Validation failed", "VALIDATION_ERROR", HttpStatusCode.BadRequest)
        {
            Errors = new Dictionary<string, string[]>
            {
                { propertyName, new[] { errorMessage } }
            };
        }
    }

    /// <summary>
    /// Thrown when entity is not found
    /// </summary>
    public class NotFoundException : DomainException
    {
        public string EntityName { get; }
        public object EntityId { get; }

        public NotFoundException(string entityName, object entityId)
            : base($"Entity {entityName} with id {entityId} was not found",
                  "NOT_FOUND",
                  HttpStatusCode.NotFound)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public NotFoundException(string message)
            : base(message, "NOT_FOUND", HttpStatusCode.NotFound)
        {
        }
    }

    /// <summary>
    /// Thrown when entity already exists
    /// </summary>
    public class AlreadyExistsException : DomainException
    {
        public string EntityName { get; }
        public string PropertyName { get; }
        public string PropertyValue { get; }

        public AlreadyExistsException(string entityName, string propertyName, string propertyValue)
            : base($"Entity {entityName} with {propertyName} '{propertyValue}' already exists",
                  "ALREADY_EXISTS",
                  HttpStatusCode.Conflict)
        {
            EntityName = entityName;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        public AlreadyExistsException(string message)
            : base(message, "ALREADY_EXISTS", HttpStatusCode.Conflict)
        {
        }
    }

    /// <summary>
    /// Thrown for business rule violations
    /// </summary>
    public class BusinessRuleException : DomainException
    {
        public string RuleName { get; }

        public BusinessRuleException(string ruleName, string message)
            : base(message, "BUSINESS_RULE_VIOLATION", HttpStatusCode.BadRequest)
        {
            RuleName = ruleName;
        }

        public BusinessRuleException(string ruleName, string message, string code)
            : base(message, code, HttpStatusCode.BadRequest)
        {
            RuleName = ruleName;
        }
    }

    /// <summary>
    /// Thrown when unauthorized access is attempted
    /// </summary>
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message)
            : base(message, "UNAUTHORIZED", HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedException()
            : base("You are not authorized to perform this action",
                  "UNAUTHORIZED",
                  HttpStatusCode.Unauthorized)
        {
        }
    }

    /// <summary>
    /// Thrown when forbidden access is attempted
    /// </summary>
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message)
            : base(message, "FORBIDDEN", HttpStatusCode.Forbidden)
        {
        }

        public ForbiddenException()
            : base("You do not have permission to perform this action",
                  "FORBIDDEN",
                  HttpStatusCode.Forbidden)
        {
        }
    }

    /// <summary>
    /// Thrown for concurrency conflicts
    /// </summary>
    public class ConcurrencyException : DomainException
    {
        public ConcurrencyException(string message)
            : base(message, "CONCURRENCY_CONFLICT", HttpStatusCode.Conflict)
        {
        }

        public ConcurrencyException(string entityName, object entityId)
            : base($"Entity {entityName} with id {entityId} has been modified since it was loaded",
                  "CONCURRENCY_CONFLICT",
                  HttpStatusCode.Conflict)
        {
        }
    }

    /// <summary>
    /// Thrown for invalid domain state transitions
    /// </summary>
    public class InvalidStateTransitionException : DomainException
    {
        public string CurrentState { get; }
        public string TargetState { get; }

        public InvalidStateTransitionException(string currentState, string targetState)
            : base($"Cannot transition from {currentState} to {targetState}",
                  "INVALID_STATE_TRANSITION",
                  HttpStatusCode.BadRequest)
        {
            CurrentState = currentState;
            TargetState = targetState;
        }

        public InvalidStateTransitionException(string message)
            : base(message, "INVALID_STATE_TRANSITION", HttpStatusCode.BadRequest)
        {
        }
    }

    /// <summary>
    /// Thrown for invalid operations on domain entities
    /// </summary>
    public class InvalidOperationDomainException : DomainException
    {
        public InvalidOperationDomainException(string message)
            : base(message, "INVALID_OPERATION", HttpStatusCode.BadRequest)
        {
        }

        public InvalidOperationDomainException(string message, string code)
            : base(message, code, HttpStatusCode.BadRequest)
        {
        }
    }

    /// <summary>
    /// Thrown for dependency violations
    /// </summary>
    public class DependencyViolationException : DomainException
    {
        public string DependentEntity { get; }
        public string RequiredEntity { get; }

        public DependencyViolationException(string dependentEntity, string requiredEntity)
            : base($"Cannot perform operation: {dependentEntity} depends on {requiredEntity}",
                  "DEPENDENCY_VIOLATION",
                  HttpStatusCode.BadRequest)
        {
            DependentEntity = dependentEntity;
            RequiredEntity = requiredEntity;
        }

        public DependencyViolationException(string message)
            : base(message, "DEPENDENCY_VIOLATION", HttpStatusCode.BadRequest)
        {
        }
    }

    /// <summary>
    /// Thrown for timeout violations
    /// </summary>
    public class TimeoutException : DomainException
    {
        public TimeoutException(string operationName, TimeSpan timeout)
            : base($"Operation {operationName} timed out after {timeout.TotalSeconds} seconds",
                  "TIMEOUT",
                  HttpStatusCode.RequestTimeout)
        {
        }

        public TimeoutException(string message)
            : base(message, "TIMEOUT", HttpStatusCode.RequestTimeout)
        {
        }
    }

}
