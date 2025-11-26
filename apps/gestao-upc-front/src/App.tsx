import './App.css'
import { ConfigProvider, theme } from 'antd'
import RouterApp from './core/router/Router'

function App() {
  return (
    <ConfigProvider
      theme={{
        algorithm: theme.defaultAlgorithm,
        token: {
          // Paleta Volcano completa
          colorPrimary: '#fa541c', // Volcano primary
          colorSuccess: '#52c41a',
          colorWarning: '#faad14',
          colorError: '#ff4d4f',
          colorInfo: '#1890ff',
          borderRadius: 6,
          // Cores de texto e links
          colorLink: '#fa541c',
          colorLinkHover: '#ff7a45',
          colorLinkActive: '#d4380d',
          // Cores de título
          colorTextHeading: '#ad2102',
          colorText: '#871400',
        },
        components: {
          Button: {
            colorPrimary: '#fa541c',
            colorPrimaryHover: '#ff7a45',
            colorPrimaryActive: '#d4380d',
          },
          Link: {
            colorLink: '#fa541c',
            colorLinkHover: '#ff7a45',
            colorLinkActive: '#d4380d',
          },
          Typography: {
            colorTextHeading: '#ad2102',
            colorText: '#871400',
          },
        },
      }}
    >
      <RouterApp />
    </ConfigProvider>
  )
}

export default App
