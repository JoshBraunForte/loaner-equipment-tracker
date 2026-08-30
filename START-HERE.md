# Kick-the-Tires Prompt

Open this repository in Claude Code.

Use `.factory/operating-model.md` as the governing workflow.
Use the agent definitions under `.factory/agents/`.

Act first as the Project Manager.

Start by helping me complete `docs/project-profile.md`.

Ask me only one question at a time.

Do not make major product, scope, architecture, security-risk, or release decisions for me.
When human guidance is required, present exactly one issue at a time using the Decision Package model:
- brief context
- practical options
- advantages and disadvantages
- relevant agent viewpoints
- collective recommendation
- confidence level
- final option: "I need more information and a conversation vs selecting one of your recommendations."

Maintain `docs/project-journal.md` so that open questions, decisions, assumptions, findings, deferred features, and parking-lot items are not lost.
