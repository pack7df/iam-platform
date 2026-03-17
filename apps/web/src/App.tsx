import './App.css'

function App() {
  return (
    <main className="app-shell">
      <section className="hero-panel">
        <p className="eyebrow">IAM Platform</p>
        <h1>Administrative web app bootstrap</h1>
        <p className="lead">
          The frontend is now running as a React + Vite SPA and is ready for the
          first management flows.
        </p>

        <div className="hero-actions">
          <a href="http://localhost:5173" target="_blank" rel="noreferrer">
            Open local app
          </a>
          <span>Task 0.4 complete: base shell online.</span>
        </div>
      </section>

      <section className="status-grid" aria-label="Initial status overview">
        <article>
          <h2>Ready now</h2>
          <p>Vite dev server, React entrypoint and a first admin shell.</p>
        </article>

        <article>
          <h2>Next frontend task</h2>
          <p>Set up Vitest and the first UI smoke test in task 0.5.</p>
        </article>

        <article>
          <h2>Expected modules</h2>
          <p>Authentication, tenants, users, roles, resources, rules and audit.</p>
        </article>
      </section>
    </main>
  )
}

export default App
