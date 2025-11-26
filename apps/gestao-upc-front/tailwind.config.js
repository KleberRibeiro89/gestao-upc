/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        // Paleta Volcano completa do Ant Design
        volcano: {
          50: '#fff1e8',
          100: '#ffd8bf',
          200: '#ffbb96',
          300: '#ff9c6e',
          400: '#ff7a45',
          500: '#fa541c', // Primary
          600: '#d4380d',
          700: '#ad2102',
          800: '#871400',
          900: '#610b00',
        },
        // Mantém amber para compatibilidade, mas mapeia para volcano
        amber: {
          50: '#fff1e8',
          100: '#ffd8bf',
          200: '#ffbb96',
          300: '#ff9c6e',
          400: '#ff7a45',
          500: '#fa541c',
          600: '#d4380d',
          700: '#ad2102',
          800: '#871400',
          900: '#610b00',
        },
      },
    },
  },
  plugins: [],
}

