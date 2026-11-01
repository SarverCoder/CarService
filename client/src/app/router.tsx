import { createBrowserRouter } from 'react-router'

import { AppLayout } from '@/components/layout/AppLayout'
import { HomePage } from '@/pages/HomePage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { PartPage } from '@/pages/PartPage'
import { PartsPage } from '@/pages/PartsPage'
import { ServicesPage } from '@/pages/ServicesPage'

export const router = createBrowserRouter([
  {
    element: <AppLayout />,
    children: [
      { index: true, element: <HomePage /> },
      { path: 'services', element: <ServicesPage /> },
      { path: 'parts', element: <PartsPage /> },
      { path: 'parts/:id', element: <PartPage /> },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
])
