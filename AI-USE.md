# AI use — DetectiveGame

AI-assisted development is permitted under the workspace policy (`../_governance/POLICY.md`). Every change must record the tools used, the human verification performed, and the data used. A human is the author of record and reviews all AI output before merge and release.

Risk tier and release gate: `../_governance/RISK-REGISTER.md`. Legal screening: `../_governance/AI-REGULATIONS.md`. This is engineering screening, not legal advice.

## Disclosure

Complete before shipping a chatbot, generative feature, or synthetic content: users must be told they are interacting with AI and generated output must be labelled. EU AI Act Article 50 duties apply since 2 August 2026.

## Human oversight

A named human reviews AI output before merge. Tier A deployments additionally require an AI impact assessment, model card, threat model or safety case, and written release sign-off.

## Data

Follow `../_governance/DATA-MAP.md`. Never send secrets, personal data, health recordings, or restricted datasets to models, hosted APIs, web tools, issues, or shared sessions.
