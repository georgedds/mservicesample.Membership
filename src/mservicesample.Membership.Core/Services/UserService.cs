using AutoMapper;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.DataAccess.Repositories;
using mservicesample.Membership.Core.Dtos.Requests;
using mservicesample.Membership.Core.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDetailsRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserDetailsRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Register(UserDto user)
        {
            var response = await _userRepository.Create(_mapper.Map<UserDetails>(user))
                .ContinueWith(x => _mapper.Map<UserDto>(x.Result));
            return response;
        }

        public async Task<Pager.PagedResult<UserDto>> GetAllUsers(Pager.PagingParams paging)
        {
            var response = _userRepository.ListPaged(paging);
            //.ContinueWith((t => _mapper.Map<List<UserDto>>(t.Result)));
            var rs = new Pager.PagedResult<UserDto>();
            rs.Results = response.Result.Results.ContinueWith((t => _mapper.Map<List<UserDto>>(t.Result)));
            rs.CurrentPage = response.Result.CurrentPage;
            rs.PageCount = response.Result.PageCount;
            rs.PageSize = response.Result.PageSize;
            rs.RowCount = response.Result.RowCount;
            // rs.FirstRowOnPage = response.Result.FirstRowOnPage;

            return rs;
        }

        public async Task<UserDto> Edit(UserDto user)
        {
            var response = await _userRepository.Update(_mapper.Map<UserDetails>(user))
                .ContinueWith(x => _mapper.Map<UserDto>(x.Result));
            return response;
        }

        public async Task<UserDto> GetById(string userid)
        {
            var response = await _userRepository.FindById(userid)
                .ContinueWith(x => _mapper.Map<UserDto>(x.Result));
            return response;
        }

        public async Task<bool> Delete(string userid)
        {
            return await _userRepository.Delete(userid);
        }
    }
}
