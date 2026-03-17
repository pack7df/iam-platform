import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'

import App from './App'

describe('App', () => {
  it('renders the admin shell headline and current milestone', () => {
    render(<App />)

    expect(
      screen.getByRole('heading', { name: 'Administrative web app bootstrap' }),
    ).toBeInTheDocument()
    expect(screen.getByText('Task 0.4 complete: base shell online.')).toBeInTheDocument()
  })
})
