using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using ToDoApp.BlazorWSM.Model;

namespace ToDoApp.BlazorWSM.Pages
{
    public partial class Index
    {
        List<ToDoModel> lst = new List<ToDoModel>();
        ToDoModel _toDo = new ToDoModel();
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
               lst = await GetData();
                StateHasChanged();
            }
        }

        private async Task Save()
        {
            lst = await GetData();
            _toDo.Id = Guid.NewGuid();
            lst.Add(_toDo);
            await Clear();

            await SetData();
            StateHasChanged();

        }

        private async Task Edit(Guid? id)
        {
            lst = await GetData();
            var item = lst.First(x => x.Id == id);
            if (item == null) return;

            var jsonStr = JsonConvert.SerializeObject(item);
            _toDo = JsonConvert.DeserializeObject<ToDoModel>(jsonStr);
            StateHasChanged();

        }

        private async Task Update()
        {
            lst = await GetData();
            var item = lst.FirstOrDefault(x=>x.Id == _toDo.Id);
            if (item == null) return;
            item.Title = _toDo.Title;
            await Clear();
            await SetData();
            StateHasChanged() ;
        }

        private async Task Delete(Guid? id)
        {
            lst = await GetData();
            var item = lst.FirstOrDefault(x => x.Id == id);
            if (item == null) return;
            lst.Remove(item);

            await SetData();
            StateHasChanged();
        }

        private async Task Clear()
        {
            _toDo = new ToDoModel();
        }
        private async Task SetData()
        {
            var jsonStr = JsonConvert.SerializeObject(lst);
            await _jsruntime.InvokeVoidAsync("localStorage.setItem", "ToDo", jsonStr);
        }
        private async Task<List<ToDoModel>> GetData()
        {
            var jsonStr = await _jsruntime.InvokeAsync<string>("localStorage.getItem", "ToDo");
            if (jsonStr == null) return new List<ToDoModel>();
            var lstData = JsonConvert.DeserializeObject<List<ToDoModel>>(jsonStr);
            return lstData;
        }
    }
}
