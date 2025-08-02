# Introduction

This document outlines the tasks and features to be implemented in the Finman application.
It serves as a roadmap for development, helping to track progress and prioritize work.

Items to be implemented will be listed at the high level only here. More detailed plans and tasks will be documented in separate PLAN-*.md files, stored in the "plans" folder. Each plan file will represent a single Analyse -> Plan -> Execute -> Review cycle.

## High-Level Features

### User Service Implementation
**Status**: Planning Complete  
**Plan Document**: [PLAN-user-service.md](plans/PLAN-user-service.md)  
**Description**: Backend user registration and management API service using .NET microservice. Includes authentication, profile management, and security features. UI will be implemented later as a unified frontend for multiple backend services.

**Phases**:
- [ ] Phase 1: Foundation setup with "Hello World"
- [ ] Phase 2: User registration functionality
- [ ] Phase 3: JWT authentication and login
- [ ] Phase 4: Protected routes and profile management
- [ ] Phase 5: Advanced user features (email verification, password reset)