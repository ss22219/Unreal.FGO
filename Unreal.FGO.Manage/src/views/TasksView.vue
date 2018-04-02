<template>
  <div class="article-wrp" v-show="!loading">
    <empty v-if="!loading && (!roles || !roles.length)" msg="您还未添加一个帐号，因此无法管理帐号。" linkName="添加帐号" url="/role"></empty>
    <empty v-else-if="!loading && (!tasks || !tasks.length)" msg="您还未添加一个计划，因此无法管理计划。" linkName="添加计划" url="/task"></empty>
    <div v-else-if="!loading && (tasks.length && this.$store.state.quests.length)">
      <div>
        <div class="article-header clearfix">
          <div class="right px-wrp clearfix">
            <div class="fq-wrp">
              <div class="dropdown">
                <a @click="add()"> <i class="iconfont icon-charge"></i><span> 添加计划</span> </a>
              </div>
            </div>
          </div>
        </div>
        <div>
          <ul>
            <li class="article-item clearfix" v-for="task in tasks">
              <div class="cover-wrp">
                <a><img v-if="getRole(task).face" class="cover-img" :src="getRole(task).face"></a>
                <div class="duration">{{getStateName(task.state)}}</div>
              </div>
              <div class="meta-wrp">
                <div class="meta-header">
                  <div class="typename">{{task.action == 'Login' ? '登陆' : '战斗'}}</div>
                  <a class="title ellipsis">{{task.name}}</a>
                  <a class="title ellipsis">{{getRole(task).username}}</a>
                  <a class="title ellipsis" v-if="!task.enable">{{task.enable ? '可用' : '已禁用'}}</a>
                  <a class="title ellipsis" v-if="task.start_time">{{startTime(task.start_time)}}</a>
                  <a class="title ellipsis" v-if="task.re_excute_count">错误重试：{{task.re_excute_count}}</a>
                </div>
                <div class="meta-body">
                  {{task.lastLog && task.lastLog.message}}
                </div>
                <div class="meta-footer">
                  <a class="edit" @click="edit(task)">
                    <i class="iconfont icon-edit"></i> 编辑 </a>

                  <a class="edit" @click="reset(task)">
                    <i class="iconfont icon-edit"></i> 重置状态</a>
                  <a @click="disable(task)" class="appeal item">{{!task.enable ? '启用' : '禁用'}}</a>
                  <a @click="logs(task)" class="appeal item">查看日志</a>
                  <a class="delete" @click="del(task)"> <i class="iconfont icon-del2"></i> </a>
                </div>
              </div>
              <div class="del-wrp" style="height: 158px" v-show="deleteId === task.id">
<div class="del-title">是否确认删除此计划？</div>
<div class="del-msg"></div>
<div class="btn-wrp">
  <a class="bili-btn ok" @click="realDel(task)">删除</a> <a class="bili-btn cancel" @click="clearDel(task)">取消</a>
</div>
</div>
</li>
</ul>
</div>
</div>
</div>
</div>
</template>

<script>
  import Empty from '../components/Empty.vue'
  export default {
    name: 'tasks-view',
    components: { Empty },
    data() {
      return {
        role: {},
        delete: false,
        deleteId: 0
      }
    },
    methods: {
      logs(task) {
        this.$router.push({ path: '/task/' + task.id + '/logs' })
      },
      startTime(time) {
        if (!time)
          return ''
        return time + ' 时启动'
      },
      reset(task) {
        this.$store.dispatch('RESET_TASK', task)
      },
      getRole(task) {
        return this.roles.find(r => r.id == task.user_role_id)
      },
      getStateName(state) {
        switch (state) {
          case -2:
            return '任务失败 游戏错误'
          case -1:
            return '任务失败 网络错误'
          case 0:
            return '等待中'
          case 1:
            return '运行中'
            break;
        }
      },
      questNames(task) {
        if (!task.quest_ids || !task.quest_ids.split(',').length)
          return '';
        var ids = task.quest_ids.split(',')
        var quests = this.$store.state.quests.filter(q => ids.find(i => i == q.id)).map(q => q.name);
        if (!quests.length)
          return
        return this.$store.state.quests.filter(q => ids.find(i => i == q.id)).map(q => q.name).reduce((i, j) => i + ' ' + j)
      },
      disable(task) {
        this.$store.dispatch('DISABLE_TASK', task)
      },
      del(task) {
        this.deleteId = task.id
      },
      realDel(task) {
        this.$store.dispatch('DELETE_TASK', task)
      },
      clearDel() {
        this.deleteId = -1
      }, edit(task) {
        this.$router.push({ path: '/task/' + task.id })
      }, add() {
        this.$router.push({ path: '/task' })
      }, loadList() {
        this.$store.dispatch('FETCH_TASKS').then(res => {
          this.$parent.loading = false
        })
      }
    },
    computed: {
      loading() {
        return this.$parent.loading
      },
      roles() {
        return this.$store.state.roles
      },
      tasks() {
        return this.$store.state.tasks
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    beforeMount() {
      this.$parent.loading = true
      if (!this.$store.state.quests.length) {
        this.$parent.hiddenBody = true
        this.$parent.showErrLoading = true
        this.$store.dispatch('FETCH_QUEST').then((res) => {
          this.$store.dispatch('FETCH_SVT').then((res) => {
            this.loadList()
          })
        })
      } else
        this.loadList()
    }
  }
</script>
<style>
  .ellipsis{
    padding-right: 10px
  }
</style>