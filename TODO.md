# Introduction

This document outlines the tasks and features to be implemented in the Finman application.
It serves as a roadmap for development, helping to track progress and prioritize work.

Items to be implemented will be listed at the high level only here. More detailed plans and tasks will be documented in separate PLAN-*.md files, stored in the "plans" folder. Each plan file will represent a single Analyse -> Plan -> Execute -> Review cycle.

## High-Level Features

### User Service Implementation
**Status**: COMPLETE ✅
**Plan Document**: [PLAN-0001-user-registration](plans/PLAN-0001-user-registration.md)  
**Completed**: August 10, 2025  
**Description**: Comprehensive user registration functionality successfully implemented for the Finman User Service. Full test coverage with 125/125 tests passing, including domain layer, application layer, infrastructure layer with BCrypt password hashing, complete API implementation, and end-to-end integration testing. All deployed and fully functional.

### PostgreSQL Persistence and Testcontainers
**Status**: IN PROGRESS ⚠️ (Phase 3/8 Complete)
**Plan Document**: [PLAN-0002-postgres-persistence-and-testcontainers](plans/PLAN-0002-postgres-persistence-and-testcontainers.md)  
**Started**: August 10, 2025  
**Last Updated**: August 15, 2025  
**Phase 3 Completed**: August 15, 2025  
**Description**: Implementing PostgreSQL persistence with EF Core while preserving Hexagonal architecture. Phase 1 (Dependencies), Phase 2 (Application Ports), and Phase 3 (Infrastructure persistence) are complete. Core infrastructure includes FinmanDbContext with sophisticated value object mappings, EF Core migrations with PostgreSQL citext support, EfUserRepository implementation, and environment-aware DI configuration. All existing functionality preserved with 114/115 tests passing. Next: Connection string configuration and Testcontainers integration tests.

