<template>
  <div class="home-wrap wide-mode" v-show="!loading">
    <div class="head-stat clearfix panel">
      <router-link class="section last-sec link-s" to="/roles">
        <i class="iconfont icon-fans"></i>
        <div class="inline">
          <div class="num">{{roleCount}}</div>
          <div class="text">帐号</div>
        </div>
      </router-link>
      <router-link class="section link-s" to="/tasks"><i class="iconfont icon-play1"></i>
        <div class="inline">
          <div class="num">{{taskCount}}</div>
          <div class="text">计划</div>
        </div>
      </router-link>
    </div>
</template>

<script>
  export default {
    name: 'home-view',
    components: {},
    computed: {
      loading() {
        return this.$parent.loading
      }, roleCount() {
        return this.$store.state.roles.length
      },
      taskCount() {
        return this.$store.state.tasks.length
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    mounted() {
    }
  }
</script>
<style>
.home-wrap .head-stat .section{
  width: 50% !important
}
</style>