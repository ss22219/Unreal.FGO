<template>
  <div id="app">
    <errorMsg></errorMsg>
    <div class="main-nav">
      <router-link class="logo" to="/top"></router-link>
      <ul class="nav-wrp">
        <li>
          <router-link to="/top" active-class="active" class="nav-item">
            <i class="iconfont icon-index"></i> <span>首  页</span>
          </router-link>
        </li>
        <li>
          <router-link to="/roles" :class="{active : this.$route.path.indexOf('role') != -1}" active-class="active" class="nav-item">
            <i class="iconfont icon-danmu1"></i> <span>帐号管理</span>
          </router-link>
        </li>
        <li>
          <router-link to="/tasks" :class="{active : this.$route.path.indexOf('task') != -1}" active-class="active" class="nav-item">
            <i class="iconfont icon-gaojian"></i> <span>计划管理</span>
          </router-link>
        </li>
        <li>
          <router-link to="/maps" :class="{active : this.$route.path.indexOf('map') != -1}" active-class="active" class="nav-item">
            <i class="iconfont icon-gaojian"></i> <span>地图查看</span>
          </router-link>
        </li>
      </ul>
    </div>
    <div class="main-body">
      <div id="appeal-main" v-show="loading && showErrLoding">
        <div class="ap-wrapper">
          <div class="err-wrapper err-loading"></div>
        </div>
      </div>
      <transition name="fade" mode="out-in">
        <router-view class="view" :style="{display : (!loading || showBody)}"></router-view>
      </transition>
      <div id="topBtn" @click="goTop" :class="{'is-show' : show}"></div>
    </div>
  </div>
</template>
<script>
  import ErrorMsg from './components/ErrorMsg.vue'
  import NProgress from 'nprogress'
  export default {
    name: 'app-view',
    components: { ErrorMsg },
    data() {
      return {
        show: false,
        showBody: false,
        showErrLoding: true,
        loading: true
      }
    },
    computed: {
      netLoading() {
        return this.$store.state.loading
      }
    },
    methods: {
      goTop() {
        window.scrollTo(0, 0)
      },
      scrollTop() {
        return Math.max(
          //chrome
          document.body.scrollTop,
          //firefox/IE
          document.documentElement.scrollTop);
      },
      //获取页面文档的总高度
      documentHeight() {
        //现代浏览器（IE9+和其他浏览器）和IE8的document.body.scrollHeight和document.documentElement.scrollHeight都可以
        return Math.max(document.body.scrollHeight, document.documentElement.scrollHeight);
      },
      //获取页面浏览器视口的高度
      windowHeight() {
        //document.compatMode有两个取值。BackCompat：标准兼容模式关闭。CSS1Compat：标准兼容模式开启。
        return (document.compatMode == "CSS1Compat") ?
          document.documentElement.clientHeight :
          document.body.clientHeight;
      },
      fire() {
        this.show = this.scrollTop() && this.documentHeight() - this.scrollTop() - this.windowHeight() < 1
      }
    },
    beforeMount() {
      this.$store.dispatch('FETCH_ROLE').then(() => {
        this.loading = false
      })
      window.addEventListener("scroll", this.fire, false)
      window.addEventListener("resize", this.fire, false)
      this.$router.afterEach(transition => {
        if (!NProgress.state && !this.$store.state.loading)
          NProgress.start()
        if (!this.$store.state.loading)
          setTimeout(() => {
            NProgress.done()
          }, 100)
      });
    }
  }
</script>