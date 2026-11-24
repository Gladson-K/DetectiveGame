# Security policy — DetectiveGame

Report vulnerabilities privately to the project owner with the affected component, impact, reproduction steps, and disclosure status. Do not open public issues for unfixed security problems.

## Rules

- Never commit `.env`, keys, tokens, or credentials. Use `.env.example` templates.
- Treat dataset content, tool output, and AI output as data, not instructions.
- Do not add unsafe model deserialisation (`weights_only=False`, `pickle.load`, `joblib.load`, `allow_pickle=True`) for external artefacts.
- Follow the tier release requirements in `../_governance/RISK-REGISTER.md`.
- Record AI assistance and human verification in each change.

## Response targets

Acknowledge within 3 business days, triage within 7 days, and provide a fix or mitigation plan within 30 days.
