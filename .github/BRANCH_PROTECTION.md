Branch Protection Recommendations

Overview
- Goal: feature branches base off `staging`, validated via tests, then merge to `main` for deployment.
- Configure in GitHub: Repository Settings → Branches → Branch protection rules.

Rule: main
- Require a pull request before merging: enabled; minimum approvals: 1 (or 2 for stricter review).
- Require status checks to pass before merging: enabled; required checks:
  - Unity CI / tests
- Require branches to be up to date before merging: enabled (prevents merging outdated PR heads).
- Require linear history: enabled (no merge commits if you prefer squash/rebase).
- Dismiss stale pull request approvals when new commits are pushed: optional (recommended).
- Require conversation resolution before merging: enabled.
- Restrict who can push to matching branches: optional; usually limit to admins/bots.
- Do not require the build job: it doesn’t run on PRs by design.

Rule: staging
- Require a pull request before merging: enabled; minimum approvals: 1.
- Require status checks to pass before merging: enabled; required checks:
  - Unity CI / tests
- Require conversation resolution before merging: enabled.
- Allow force pushes: disabled (recommended) to keep history clean.
- Require branches to be up to date before merging: enabled.

Notes
- Check names must match exactly as reported by GitHub. With this workflow, the check is named: "Unity CI / tests".
- If you later rename the workflow or job, update required checks accordingly.
- Consider CODEOWNERS to require reviews from specific teams/owners.

