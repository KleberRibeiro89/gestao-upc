module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#f59e42', // amber-500
          ...require('tailwindcss/colors').amber,
        },
      },
    },
  },
  plugins: [
    require('tailwindcss-clip-path'),
    require('tailwindcss-merge-classes'),
    require('tailwindcss-nesting'),
  ],
}