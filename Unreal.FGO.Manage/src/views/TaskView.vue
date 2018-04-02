<template>
  <div>
    <div>
      <div class="danmu-wrp">
        <div class="danmu-header"> <span class="main-title">{{task.id ? "编辑" : "添加"}}计划</span> <span class="second-title">(战斗计划包含了登陆计划，同一时间不能有多个运行)</span>
          <a class="bili-btn" :class="{ok : validate}" @click="save">保存</a>
        </div>
        <div class="section-wrp">
          <div class="section clearfix first">
            <div class="left label">
              <div class="title"> <span class="main">执行帐号</span> </div>
            </div>
            <div class="left content">
              <div>
                <select class="bili-input" v-model="roleId">
                  <option v-for="(role,index) in roles" :value="role.id">{{role.username}}</option>
                </select>
              </div>
            </div>
          </div>
          <div class="section clearfix type">
            <div class="left label">
              <div class="title"> <span class="main">计划类型</span> </div>
            </div>
            <div class="left content">
              <div class="input-group"> <label> <input type="radio" name="type" v-model="action" class="bili-radio" value="Login"> <span>登陆</span> </label>                </div>
              <div class="input-group last"> <label> <input type="radio" name="type" v-model="action" class="bili-radio" value="Battle"> <span>副本战斗</span> </label>
                <div class="input-group-inner" v-show="action == 'Battle'"> <label> <input v-model="useItem" type="checkbox" class="bili-checkbox"> <span>使用苹果</span> </label> </div>
              </div>
            </div>
          </div>
          <div class="section clearfix" v-show="action == 'Battle'">
            <div class="left label">
              <div class="title"> <span class="main">编队</span> </div>
            </div>
            <div class="left content">
              <div>
                <select class="bili-input" v-model="deckId">
                  <option v-for="(deck,index) in [0,1,2,3,4,5,6]" :value="index">第{{index + 1}}编队</option>
                </select>
              </div>
            </div>
          </div>
          <div class="section clearfix" v-show="action == 'Battle'">
            <div class="left label">
              <div class="title"> <span class="main">复制计划</span> </div>
            </div>
            <div class="left content">
              <div>
                <select class="bili-input" v-model="cloneTaskId">
                  <option v-for="(task,index) in tasks" :value="task.id">{{task.name}}</option>
                </select>
                <a class="bili-btn ok" @click="cloneTask">复制</a>
              </div>
            </div>
          </div>
          <div class="section clearfix" v-show="action == 'Battle'">
            <div class="left label">
              <div class="title"> <span class="main">队友选择</span> </div>
              <div class="title-tip"> 优先选择列表第一个可以战斗的队友，匹配不到时选择第一个可用队友 </div>
            </div>
            <div class="left content">
              <div>
                <searchBox :data="svtNames" @enter="addFollower" v-model="follower"></searchBox> <a class="bili-btn ok" @click="addFollower">添加</a> </div>
              <ul class="filter-wrp clearfix">
                <li class="filter-item" v-for="item in followers"> <span>{{item.name}}</span> <i @click="removeFollower(item)" class="iconfont icon-del1 filter-del"></i> </li>
              </ul>
            </div>
          </div>
          <div class="section clearfix last" v-show="action == 'Battle'">
            <div class="left label">
              <div class="title"> <span class="main">战斗副本</span> </div>
              <div class="title-tip"> 优先选择列表第一个可以战斗的副本，体力不足时会继续匹配，帐号中查看不到的副本不会进行战斗 </div>
            </div>
            <div class="left content">
              <div>
                <searchBox :data="questNames" @enter="addQuests" v-model="quest"></searchBox> <a class="bili-btn ok" @click="addQuests">添加</a> </div>
              <ul class="filter-wrp clearfix">
                <li class="filter-item" v-for="item in quests"> <span>{{item.name}}</span> <i @click="removeQuest(item)" class="iconfont icon-del1 filter-del"></i> </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  </div>
</template>

<script>
  import SearchBox from '../components/SearchBox.vue'
  export default {
    name: 'task-view',
    components: { SearchBox },
    data() {
      return {
        cloneTaskId: 0,
        id: 0,
        useItem: false,
        deckId: '',
        follower: '',
        followerid: '',
        followers: [],
        svtNames: [],
        questNames: [],
        quests: [],
        quest: '',
        roleId: 0,
        task: {},
        loading: false,
        action: 'Login'
      }
    },
    methods: {
      cloneTask() {
        var task = this.tasks.find(t => t.id == this.cloneTaskId)
        if (task) {
          if (task.quest_ids) {
            var ids = task.quest_ids.split(',');
            if (ids && ids.length)
              ids.forEach(questId => {
                if (!this.quests.find(q => q.id == questId)) {
                  this.quests.push(this.$store.state.quests.find(q => q.id == questId))
                }
              })
          }
          if (task.follower_id) {
            var ids = task.follower_id.split(',');
            if (ids && ids.length)
              ids.forEach(fId => {
                if (!this.followers.find(q => q.id == fId)) {
                  this.followers.push(this.$store.state.svts.find(q => q.id == fId))
                }
              })
          }
        }
      },
      addQuests() {
        var quest = this.$store.state.quests.find(q => q.name == this.quest)
        if (quest && !this.quests.find(i => quest.id == i.id)) {
          this.quest = this.quest.substring(0, this.quest.length - 1)
          this.quests.push(quest)
          this.questNames = this.$store.state.quests.filter(q => !this.quests.find(n => q.name == n.name)).map(q => q.name)
        }
      },
      removeQuest(quest) {
        var i = this.quests.indexOf(quest)
        if (i != -1)
          this.quests.splice(i, 1)
      },
      addFollower() {
        var svt = this.$store.state.svts.find(q => q.name == this.follower)
        if (svt && !this.followers.find(i => svt.id == i.id)) {
          this.follower = this.follower.substring(0, this.follower.length - 1)
          this.followers.push(svt)
          this.svtNames = this.$store.state.svts.filter(q => !this.followers.find(n => q.name == n.name)).map(q => q.name)
        }
      },
      removeFollower(svt) {
        var i = this.followers.indexOf(svt)
        if (i != -1)
          this.followers.splice(i, 1)
      },
      save() {
        if (this.validate) {
          var task = this.task
          task.id = this.id
          task.action = this.action
          if (this.quests.length)
            task.quest_ids = this.quests.map(q => q.id).reduce((i, j) => i + ',' + j)
          if (this.followers.length)
            task.follower_id = this.followers.map(q => q.id).reduce((i, j) => i + ',' + j)
          task.user_role_id = this.roleId
          task.useitem = this.useItem
          task.deckid = this.deckId
          this.$store.dispatch('SET_TASK', task).then(() => {
            this.$router.push({ path: '/tasks' })
          })
        }
      }
    },
    computed: {
      tasks() {
        return this.$store.state.tasks.map(t => {
          t.name = this.$store.state.roles.find(r => r.id == t.user_role_id).username
          return t
        })
      },
      validate() {
        return this.roleId && this.action && (this.action != "Battle" || this.quests.length)
      }, roles() {
        return this.$store.state.roles
      }
    },
    // on the server, only fetch the item itself
    //preFetch: fetchItem,
    // on the client, fetch everything
    beforeMount() {
      this.$parent.loading = true
      if(!this.roles.length){
        this.$router.back()
        return
      }
      if (this.$route.params.id) {
        if (this.$route.params.id.indexOf('r') != -1) {
          this.roleId = this.$route.params.id.replace('r', '')
        } else {
          var task = this.$store.state.tasks.find(r => r.id == this.$route.params.id);
          var questIds = [];
          var followerids = [];
          if (task) {
            this.id = task.id
            this.task = task
            this.action = task.action
            this.roleId = task.user_role_id
            this.followerid = task.follower_id
            questIds = task.quest_ids ? task.quest_ids.split(',') : questIds
            followerids = task.follower_id ? task.follower_id.split(',') : followerids
            this.quests = this.$store.state.quests.filter(q => questIds.find(i => i == q.id))
            this.followers = this.$store.state.svts.filter(s => followerids.find(i => i == s.id))
            this.useItem = task.useitem
            this.deckId = task.deckid
          }
        }
      }
      if (!this.roleId && this.roles.length)
        this.roleId = this.roles[0].id
      if (!this.$store.state.quests.length)
        this.$store.dispatch('FETCH_QUEST').then((res) => {
          this.questNames = res.data.data.filter(q => !this.quests.find(n => q.name == n.name)).map(q => q.name)
        })
      else
        this.questNames = this.$store.state.quests.filter(q => !this.quests.find(n => q.name == n.name)).map(q => q.name)

      if (!this.$store.state.svts.length) {
        this.$parent.hiddenBody = true
        this.$parent.showErrLoading = true
        this.$store.dispatch('FETCH_SVT').then((res) => {
          this.svtNames = res.data.data.filter(f => !this.followers.find(n => n.name == f.name)).map(f => f.name)
          this.$parent.loading = false
        })
      }
      else {
        this.svtNames = this.$store.state.svts.filter(f => !this.followers.find(n => n.name == f.name)).map(f => f.name)
        this.$parent.loading = false
      }
    }
  }
</script>
<style>
.meta-body{
      text-overflow: ellipsis;
    overflow: hidden;
    white-space: nowrap;
    width: 650px;
}
</style>