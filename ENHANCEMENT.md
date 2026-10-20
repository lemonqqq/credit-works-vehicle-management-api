# Future enhancements

These are potential changes after the assessment's core workflows are complete. They are not claims about current behavior.

## Assisted category range adjustment

When an administrator moves a boundary, the editor could move the adjacent category's matching boundary automatically. For example, moving Medium's upper bound from 2500 to 2000 kg would also move Heavy's lower bound to 2000 kg. For category insertion or deletion, offer a preview of the proposed complete range set and affected vehicle counts. Reject ambiguous edits until the administrator chooses which neighbour absorbs a gap or overlap. Preserve the server's final full-set validation and atomic save; automatic adjustment should never silently change unrelated ranges.

## Regional and multiple-language support

Externalize UI copy and validation messages, select a locale, and format numbers and dates for that locale. Keep API weights in kilograms with decimal values and define whether alternate display units are allowed; convert only at the presentation boundary. Clarify accepted characters in names and manufacturer search before internationalization, since current ASCII-oriented validation would reject many real names. Add translated icon labels and locale-aware accessibility text.

## Local reliability and queue options

Keep the default single-machine setup simple. If background writes remain useful, evaluate a small database-backed job table or another lightweight queue library with persistence, retries, expiry, and failure detail. The queue abstraction should allow a synchronous command path where background processing provides no user benefit. Add status retention and a clear recovery policy for restarts. Shared brokers and multi-instance cache coordination would be needed only if deployment requirements expand beyond one process.

## Other extensions

- Expose manufacturer choices from the API and later support controlled manufacturer administration.
- Make icon choices data driven while preserving stable keys and a safe fallback.
- Move sorting into the SQL query used for pagination and return the selected order; add tests across multiple pages.
- Add accessible form feedback, keyboard review, and a clear completion/error state for queued writes.
- Add optimistic concurrency for category edits and database constraints for individual values and uniqueness where appropriate.
- Extend health checks to report database readiness separately from host availability, with production observability if deployed.
