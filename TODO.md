# MyLibrary.JsonRepositoryModel - Implementation Plan

## Phase 1: Project Setup
- [ ] Create new .NET project MyLibrary.JsonRepositoryModel
- [ ] Add reference to MyLibrary.DbModel project
- [ ] Add System.Text.Json package reference

## Phase 2: Core Infrastructure
- [ ] Create JsonDbContext class to read from JSON files
- [ ] Create base Repository<T> class for JSON data access

## Phase 3: Repository Interfaces
- [ ] Create IRepository<T> interface
- [ ] Create IBookRepository interface
- [ ] Create IAuthorRepository interface
- [ ] Create IUserRepository interface
- [ ] Create IThemeRepository interface
- [ ] Create IBookAuthorRepository interface
- [ ] Create IBookCopyRepository interface
- [ ] Create IBookThemeRepository interface
- [ ] Create IBorrowRepository interface

## Phase 4: Repository Implementations
- [ ] Create BookRepository with custom methods
- [ ] Create AuthorRepository with custom methods
- [ ] Create UserRepository with custom methods
- [ ] Create ThemeRepository
- [ ] Create BookAuthorRepository
- [ ] Create BookCopyRepository
- [ ] Create BookThemeRepository
- [ ] Create BorrowRepository
