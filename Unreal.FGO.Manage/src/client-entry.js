import 'es6-promise/auto'
import { app, router, store } from './app'
import vue from 'vue'
import NProgress from 'nprogress'
import resource from 'vue-resource'
vue.use(resource)
vue.http.options.crossOrigin = true
var token = sessionStorage.getItem('token')
if (!token)
  location.href = '/public/login.html'
vue.http.interceptors.push((request, next) => {
  if (!NProgress.state)
    NProgress.start()
  if (request.url.indexOf('.json') != -1 || request.url.indexOf('user/') != -1) {
    request.headers.delete("Authorization")
  } else {
    request.headers.set("Authorization", token)
  }
  next((response) => {
    NProgress.done()
    if (response.ok && response.body.code == 88)
      location.href = '/public/login.html'
  })
})
// prime the store with server-initialized state.
// the state is determined during SSR and inlined in the page markup.
// actually mount to DOM
app.$mount('#app')

// service worker
if (process.env.NODE_ENV === 'production' && 'serviceWorker' in navigator) {
  navigator.serviceWorker.register('/service-worker.js')
}
