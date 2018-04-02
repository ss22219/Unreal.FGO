<template>
  <div class="log-warp" v-show="!loading">
    <div class="log-list">
      <div v-for="(log,index) in logs" class="log-item">
        <div class="log-header">
          <span class="left">{{actionName(log.action)}}</span>
          <span class="right">{{log.create_time}}</span>
        </div>
        <p class="detail">{{log.message}}</p>
      </div>
    </div>
  </div>
</template>
<script>
  export default {
    name: 'task-logs-view',
    data() {
      return {
        logs: []
      }
    },
    methods: {
      actionName(action) {
        switch (action) {
          case 0:
            return '无';
          case 3:
            return '检查版本';
          case 4:
            return 'b站登陆';
          case 5:
            return 'b站登陆';
          case 6:
            return '登陆';
          case 7:
            return '登陆';
          case 8:
            return '登陆';
          case 9:
            return '首页';
          case 10:
            return '开始战斗';
          case 11:
            return '结束战斗';
          case 12:
            return '礼物列表';
          case 13:
            return '领取礼物';
          case 14:
            return '使用物品';
          case 15:
            return '设置队伍';
          case 1024:
            return '启动计划';
          default:
            return '未知'
        }
      }
    },
    computed: {
      loading() {
        return this.$parent.loading
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    beforeMount() {
      this.$parent.loading = true
      this.$parent.hiddenBody = true
      this.$parent.showErrLoading = true
      this.$store.dispatch('FETCH_TASK_LOGS', this.$route.params.id).then(res => {
        this.$parent.loading = false
        this.logs = res.data.data
      })
      // fetchItemAndComments(this.$store).then(() => {
      //   this.loading = false
      // })
    }
  }
</script>
<style>
.log-warp{
  color:#505050
}
  .log-list{
    width: 680px;
    margin: 0 auto;
  }
  .log-list .log-item{
    border-bottom: 1px solid #eee;
    background-color: #fff;
    padding: 20px;
  }
  .log-list .left{
    padding: 0 8px;
    border: 1px solid #e5e9ef;
    color: #99a2aa;
    border-radius: 12px;
    vertical-align: top;
    height: 24px;
    line-height: 22px;
  }
  .log-list .log-item:hover{
    color: #00a1d6!important
  }
  .log-list .log-item .item-header{
    height: 20px;
    font-size: 14px
  }
  .log-list .log-item .detail{
    padding-top: 35px;
    text-indent: 4em;
    font-size: 18px;
  }
</style>