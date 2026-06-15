# Noesis

> **A Shared Long-Term Memory System for AI Agents**
>
> Build once. Remember forever. Collaborate across models.

---

<p align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![C++](https://img.shields.io/badge/C%2B%2B-17-blue)
![SQLite](https://img.shields.io/badge/SQLite-FTS5-green)
![Platform](https://img.shields.io/badge/Linux-macOS-orange)
![Status](https://img.shields.io/badge/Status-MVP%20Development-yellow)

</p>

---

## Overview

**Noesis** is a memory operating system for AI.

It provides a **shared long-term brain** that allows multiple AI agents running on the same machine to:

* Remember past work
* Share decisions
* Learn from previous sessions
* Build knowledge over time
* Hand off tasks between models without losing context

Instead of relying on fragile conversation history, Noesis captures AI sessions, extracts structured knowledge, stores it permanently, and injects relevant memories back into future sessions.

The result is a system where:

> Claude can learn something today,
> Gemini can use it tomorrow,
> and a local model can build on it next week.

---

## The Problem

Modern AI assistants suffer from a fundamental limitation:

* Context windows are temporary
* Sessions are isolated
* Knowledge disappears after conversations end
* Different models cannot naturally collaborate

Developers repeatedly re-explain the same project information to multiple models.

Important decisions get lost.

Lessons learned vanish.

Architectural knowledge must be rediscovered again and again.

---

## The Noesis Solution

Noesis sits between the user and AI systems.

Instead of asking models to manage memory themselves, Noesis observes their interactions externally.

The AI remains unchanged.

The memory becomes persistent.

```text
User
 │
 ▼
Noesis
 │
 ├─ Captures sessions
 ├─ Extracts knowledge
 ├─ Stores memory
 ├─ Indexes information
 └─ Injects relevant context
 │
 ▼
Claude / Gemini / Codex / Local Models
```

The AI never needs to know Noesis exists.

---

# Key Features

## Shared Memory Across Agents

Multiple AI systems can collaborate using a common memory layer.

Supported MVP agents:

* Claude Code
* Gemini CLI
* Codex CLI
* LM Studio models
* OpenAI-compatible local model servers
* Future CLI-based agents

---

## Persistent Knowledge Storage

Noesis stores:

* Decisions
* Lessons learned
* Stable knowledge
* Engineering patterns
* Project entities
* Relationships

All stored as human-readable Markdown.

---

## Full-Text Semantic Retrieval

Powered by SQLite FTS5:

* Fast local search
* BM25 ranking
* Zero infrastructure
* Cross-platform

```sql
SELECT *
FROM memory
WHERE memory MATCH 'database concurrency'
ORDER BY bm25(memory);
```

---

## Sequential Multi-Agent Handoff

One model can continue where another stopped.

Example:

```text
Claude:
Design database schema

↓ Memory Extracted

Gemini:
Implement API using schema

↓ Memory Extracted

Local Model:
Generate tests using implementation
```

No manual copy-pasting required.

---

## Transparent Session Capture

### CLI Agents

Captured through a native PTY wrapper.

```text
User Terminal
      │
      ▼
 Noesis PTY
      │
      ▼
 Claude / Gemini / Codex
```

No process modification.

No model tampering.

No API hacking.

---

### Local Models

Captured through an HTTP reverse proxy.

```text
Application
     │
     ▼
Noesis Proxy
     │
     ▼
LM Studio
```

All prompts and responses are recorded automatically.

---

# Architecture

```text
                 ┌─────────────────────┐
                 │      User           │
                 └──────────┬──────────┘
                            │
                            ▼
              ┌─────────────────────────┐
              │         Noesis          │
              └──────────┬──────────────┘
                         │
         ┌───────────────┼────────────────┐
         ▼               ▼                ▼

  Session Capture   Context Builder   Extraction Pipeline
         │               │                │
         ▼               ▼                ▼

      Raw Logs      Memory Context    Structured Memory
         │               │                │
         └───────────────┼────────────────┘
                         ▼

                 SQLite FTS5 Index

                         ▼

                    Brain Storage
```

---

# The Brain

The Brain is Noesis's permanent memory layer.

```text
brain/
├── raw/
├── decisions/
├── lessons/
├── knowledge/
├── patterns/
├── entities/
├── relationships/
└── manifests/
```

Each memory entry is stored as Markdown with YAML metadata.

Example:

```yaml
---
id: 3f7a8c2d
type: decision
project: noesis
confidence: high
created: 2026-06-14T10:32:00Z
---
```

```markdown
# Decision

Use PostgreSQL for concurrent workloads.

Reason:
SQLite locking became a bottleneck.
```

---

# Memory Lifecycle

```text
AI Session
     │
     ▼
Capture Transcript
     │
     ▼
Heuristic Extraction
     │
     ▼
Model Extraction
     │
     ▼
Trust Scoring
     │
     ▼
Validation
     │
     ▼
Brain Storage
     │
     ▼
SQLite Index
     │
     ▼
Future Context Injection
```

---

# Extraction Pipeline

Noesis uses a hybrid extraction strategy.

## Heuristic Extraction

Regex-based pattern detection:

* Decisions
* Lessons
* Entities

Example:

```text
"We decided to use PostgreSQL..."
```

↓

```json
{
  "type": "decision"
}
```

---

## Model-Based Extraction

A local model independently extracts:

```json
{
  "decisions": [],
  "lessons": [],
  "entities": []
}
```

---

## Trust Scoring

Confidence increases when both systems agree.

| Agreement  | Confidence |
| ---------- | ---------- |
| Both Match | High       |
| One Match  | Medium     |
| Neither    | Discard    |

---

# Technology Stack

## Core Platform

* .NET 8
* C# 12
* SQLite FTS5
* ASP.NET Core

---

## Native Layer

* C++17
* POSIX PTY APIs
* forkpty()
* openpty()
* waitpid()

---

## Storage

* Markdown
* YAML Frontmatter
* SQLite

---

## CLI Experience

* Spectre.Console
* System.CommandLine

---

# Project Structure

```text
noesis/
│
├── src/
│   ├── Noesis.Core/
│   ├── Noesis.Capture/
│   ├── Noesis.Extraction/
│   ├── Noesis.Context/
│   └── Noesis.CLI/
│
├── native/
│   └── pty-wrapper/
│
├── brain/
│
├── projects/
│
├── skills/
│
├── noesis.db
├── noesis.json
└── README.md
```

---

# Example Workflow

## Create a Project

```bash
noesis init my-project
```

---

## Run a Task

```bash
noesis task "Design database schema" --agent=claude
```

Noesis:

* Builds context
* Injects memory
* Launches Claude
* Captures session
* Extracts knowledge

---

## Continue with Another Agent

```bash
noesis task "Implement API" --agent=gemini --handoff
```

Gemini immediately receives all relevant project memory.

---

## Search the Brain

```bash
noesis brain search "database"
```

Output:

```text
Decision   High     Use PostgreSQL
Lesson     High     SQLite locking issue
Knowledge  Medium   PostgreSQL supports MVCC
```

---

# Roadmap

### Phase 0

Core models, storage, SQLite indexing

### Phase 1

CLI framework and project initialization

### Phase 2

Brain writing and retrieval

### Phase 3

PTY capture subsystem

### Phase 4

HTTP proxy capture

### Phase 5

Extraction pipeline

### Phase 6

Context builder

### Phase 7

Multi-agent handoff

### Phase 8

Brain manifests and summarization

### Phase 9

Advanced retrieval and future research

---

# 🎓 Design Principles

## The AI Never Manages Memory

Noesis manages memory.

The model performs work.

---

## Raw Data Is Sacred

Captured sessions are never modified.

They can always be reprocessed.

---

## Markdown Is The Source Of Truth

The database is an index.

The files are the knowledge.

---

## Human Readable First

Every memory entry remains understandable without proprietary tooling.

---

## Local First

No cloud dependency is required.

No vendor lock-in.

---

# What Noesis Is Not

Noesis is not:

* A chatbot
* An AI wrapper
* An agent framework
* A vector database
* A replacement for Claude Code
* A replacement for Gemini CLI
* An autonomous AI system

Noesis is a memory layer.

Nothing more.

Nothing less.

---

# Vision

The long-term goal of Noesis is to provide a persistent cognitive layer for AI development workflows.

A future where:

* AI agents remember projects for years
* Knowledge survives across sessions
* Multiple models collaborate naturally
* Context becomes cumulative rather than disposable

Noesis transforms AI from a collection of isolated conversations into a continuously learning engineering system.

---

## License

License to be determined.

---

## Contributing

Contributions, feedback, architecture discussions, and experimentation are welcome.

If you're interested in AI memory systems, agent collaboration, retrieval architectures, or developer tooling, feel free to open an issue or start a discussion.

---

<p align="center">
<strong>Build once. Remember forever.</strong><br>
Noesis
</p>
